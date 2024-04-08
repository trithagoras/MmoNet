using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Shared.Packets;
using MmoNet.Core.ServerApp.Exceptions;
using MmoNet.Core.States;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MmoNet.Core.ServerApp; 
public class ServerApplication(IProtocolLayer protocolLayer,
    ISerializer serializer,
    ISessionManager sessionManager,
    ILogger<ServerApplication> logger,
    IPacketRegistry packetRegistry,
    IServiceProvider serviceProvider,
    IExceptionFilter exceptionFilter) {

    public int TickRate { get; private set; }

    // Maps packet id to tuple of controller action and required state if applied
    Dictionary<int, (MethodInfo, Type?)> ActionMap { get; set; } = [];

    readonly IServiceProvider serviceProvider = serviceProvider;
    readonly IProtocolLayer protocolLayer = protocolLayer;
    readonly ISerializer serializer = serializer;
    readonly ISessionManager sessionManager = sessionManager;
    readonly ILogger<ServerApplication> logger = logger;
    readonly IPacketRegistry packetRegistry = packetRegistry;
    readonly IExceptionFilter exceptionFilter = exceptionFilter;
    readonly ConcurrentDictionary<Guid, IPacket> incomingPackets = new();
    readonly ConcurrentDictionary<Guid, IPacket> outgoingPackets = new();

    public async Task StartAsync(int port, int tickRate) {
        TickRate = tickRate;
        protocolLayer.OnConnected += NewConnection;
        protocolLayer.OnDisconnected += Disconnection;
        protocolLayer.OnPacketSent += PacketSent;
        protocolLayer.OnPacketReceived += (_, p) => HandleIncomingPacket(p);
        protocolLayer.OnException += Exception;

        // begin tick task
        _ = Task.Run(StartTickLoop);
        await protocolLayer.StartAsync(port);
    }

    public async Task StopAsync() {
        await protocolLayer.StopAsync();
    }

    public ServerApplication MapControllers() {
        var controllers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Controller)))
            .Select(t => ActivatorUtilities.CreateInstance(serviceProvider, t) as Controller)
            .ToList();

        // each controller action should have a mapping to a unique IPacket implementation
        var map = new Dictionary<int, (MethodInfo, Type?)>();
        controllers?.ForEach(c => {
            var controllerType = c.GetType();
            var controllerActions = controllerType.GetMethods()
                .Where(m => m.IsPublic &&
                m.ReturnType == typeof(Task<IPacket>) || m.ReturnType == typeof(IPacket))
                .ToList();

            controllerActions.ForEach(a => {
                var parameters = a.GetParameters();
                var param = parameters.SingleOrDefault(p => p.ParameterType.GetInterface(nameof(IPacket)) != null) ?? throw new InvalidOperationException($"Controller action {a.Name} does not have a single parameter of type IPacket."); ;
                var p = Activator.CreateInstance(param.ParameterType) as IPacket;
                if (map.TryGetValue(p.PacketId, out var value)) {
                    throw new InvalidOperationException($"Attempted to map Packet type {p.GetType()} to controller action {c}.{a.Name}, but is already mapped to controller action {c}.{value.Item1.Name}.");
                }
                // check if the controller action has a RequiresState attribute
                var requiresStateAttribute = a.GetCustomAttributes()
                    .Where(attr => attr.GetType().IsGenericType && attr.GetType().GetGenericTypeDefinition() == typeof(RequiresStateAttribute<>))
                    .SingleOrDefault();
                if (requiresStateAttribute != null) {
                    var stateType = requiresStateAttribute.GetType().GetGenericArguments()[0];
                    map.Add(p.PacketId, (a, stateType));
                    return;
                }
                map.Add(p.PacketId, (a, null));
            });
        });

        ActionMap = map;

        return this;
    }

    public void RegisterPackets() {
        var map = new Dictionary<int, Type>();

        var packetTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IPacket).IsAssignableFrom(t)))
            .ToList();

        foreach (var t in packetTypes) {
            var packetIdAttr = t.GetCustomAttribute<PacketIdAttribute>()
                ?? throw new InvalidOperationException($"Packet type {t.Name} does not have a PacketId attribute.");
            if (map.TryGetValue(packetIdAttr.Id, out Type? value)) {
                var existingType = value;
                throw new InvalidOperationException($"Attempted to assign packet id {packetIdAttr.Id} to {t.Name}, but is already assigned to packet type {existingType}.");
            }
            map.Add(packetIdAttr.Id, t);
        }

        packetRegistry.RegisterPackets(map);
    }

    async Task DispatchPacket(IPacket packet) {
        ISession session = null!;
        try {
            session = sessionManager[packet.SessionId];
        } catch (KeyNotFoundException) {
            // log warning
            logger.LogWarning("Received packet with invalid session id {sessionId}", packet.SessionId);
            return;
        }

        logger.LogInformation("Packet received from session {session}: {packet}", session.Id, packet);

        if (!ActionMap.TryGetValue(packet.PacketId, out var tuple)) {
            logger.LogWarning("Received packet with invalid packet id.");
            var exceptionContext = new ActionExceptionContext(session, packet, new InvalidStateException($"Attempted to send unregistered packet {packet} in state {session.State}"));
            Exception(this, exceptionContext);
            return;
        }
        var (value, stateType) = tuple;

        // check if session is in the correct state (as defined by the controller action)
        var controllerType = value.DeclaringType;
        if (stateType != null) {
            if (session.State.GetType() != stateType) {
                logger.LogWarning("Session {id} attempted to send a packet ({p}) in an unregistered state ({state})", session.Id, packet, session.State);
                var exceptionContext = new ActionExceptionContext(session, packet, new InvalidStateException($"Attempted to send unregistered packet {packet} in state {session.State}"));
                Exception(this, exceptionContext);
                return;
            }
        }

        var controller = serviceProvider.GetRequiredService(value.DeclaringType!) as Controller;
        try {
            IPacket response = null!;
            if (value.GetParameters().Any(p => p.GetCustomAttribute<FromSessionAttribute>() != null)) {
                response = await (Task<IPacket>)value.Invoke(controller, new object[] { packet, session })!;
            } else {
                response = await (Task<IPacket>)value.Invoke(controller, new object[] { packet })!;
            }
            await protocolLayer.SendAsync(session, response);
        } catch (Exception e) {
            var exceptionContext = new ActionExceptionContext(session, packet, e);
            exceptionFilter.OnException(exceptionContext);
        }
    }

    async Task StartTickLoop() {
        var desiredTickInterval = TimeSpan.FromMilliseconds(1000.0 / TickRate);
        var stopwatch = Stopwatch.StartNew();

        while (true) {
            stopwatch.Restart();
            await TickAsync();

            var elapsed = stopwatch.Elapsed;
            var diff = desiredTickInterval - elapsed;

            if (diff > TimeSpan.Zero) {
                await Task.Delay(diff);
            } else if (diff < TimeSpan.Zero) {
                logger.LogWarning("Tick time budget exceeded by {diff} milliseconds", -diff.TotalMilliseconds);
            }
        }
    }

    // Current implementation means that the server will only handle one packet per session per tick
    void HandleIncomingPacket(IPacket packet) {
        incomingPackets[packet.SessionId] = packet;
    }

    async Task TickAsync() {
        foreach (var packet in incomingPackets.Values) {
            await DispatchPacket(packet);
        }
        incomingPackets.Clear();
    }

    void Disconnection(object? sender, ISession session) {
        sessionManager.RemoveSession(session.Id);
        logger.LogInformation("Client {sessionId} has disconnected.", session.Id);
    }

    void NewConnection(object? sender, ISession session) {
        sessionManager[session.Id] = session;
        logger.LogInformation("Client {sessionId} has connected.", session.Id);
        AssignId(session);
    }

    void AssignId(ISession session) {
        var packet = new IdPacket() {
            SessionId = session.Id
        };
        protocolLayer.SendAsync(session, packet);
    }

    void PacketSent(object? sender, IPacket packet) {
        logger.LogInformation("Sent packet to session {session}: {packet}", packet.SessionId, packet);
    }

    void Exception(object? sender, ActionExceptionContext ctx) {
        exceptionFilter.OnException(ctx);
    }
}
