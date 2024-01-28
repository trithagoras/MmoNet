using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MmoNet.Core.Middlewares;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Shared.Packets;
using static System.Collections.Specialized.BitVector32;

namespace MmoNet.Core.ServerApp; 
public class ServerApplication(IProtocolLayer protocolLayer,
    ISerializer serializer,
    ISessionManager sessionManager,
    ILogger<ServerApplication> logger,
    IPacketRegistry packetRegistry,
    IServiceProvider serviceProvider) {
    public Dictionary<int, MethodInfo> ActionMap { get; private set; } = [];

    readonly IServiceProvider serviceProvider = serviceProvider;
    readonly IProtocolLayer protocolLayer = protocolLayer;
    readonly ISerializer serializer = serializer;
    readonly ISessionManager sessionManager = sessionManager;
    readonly ILogger<ServerApplication> logger = logger;
    readonly IPacketRegistry packetRegistry = packetRegistry;
    readonly List<IMiddleware> middlewares = [];

    public async Task StartAsync(int port) {
        protocolLayer.OnConnected += NewConnection;
        protocolLayer.OnDisconnected += Disconnection;
        protocolLayer.OnPacketSent += PacketSent;
        await protocolLayer.StartAsync(port);
    }

    public async Task StopAsync() {
        await protocolLayer.StopAsync();
    }

    public void AddMiddleware<T>() where T : IMiddleware, new() {
        // Check if a middleware of type T already exists in the list
        if (middlewares.Any(m => m.GetType() == typeof(T))) {
            throw new InvalidOperationException($"Middleware of type {typeof(T).Name} is already added.");
        }
        middlewares.Add(new T());
    }

    public void RunDeferred(int ms, Func<Task> action) {
        var task = Task.Delay(ms).ContinueWith(async _ => {
            await action();
        });
    }

    public ServerApplication MapControllers() {
        var controllers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Controller)))
            .Select(t => ActivatorUtilities.CreateInstance(serviceProvider, t) as Controller)
            .ToList();

        // each controller action should have a mapping to a unique IPacket implementation
        var map = new Dictionary<int, MethodInfo>();
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

                if (map.TryGetValue(p.PacketId, out MethodInfo? value)) {
                    throw new InvalidOperationException($"Attempted to map Packet type {p.GetType()} to controller action {c}.{a.Name}, but is already mapped to controller action {c}.{value.Name}.");
                }
                map.Add(p.PacketId, a);
            });
        });

        ActionMap = map;
        protocolLayer.OnPacketReceived += DispatchPacket;

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

    void DispatchPacket(object? sender, IPacket packet) {
        ISession session = null!;
        try {
            session = sessionManager[packet.SessionId];
        } catch (KeyNotFoundException) {
            // log warning
            logger.LogWarning("Received packet with invalid session id {sessionId}", packet.SessionId);
            return;
        }

        logger.LogInformation("Packet received from session {session}: {packet}", session.Id, packet);


        if (!ActionMap.TryGetValue(packet.PacketId, out MethodInfo? value)) {
            logger.LogWarning("Received packet with invalid packet id.");
            return;
        }

        var controller = serviceProvider.GetRequiredService(value.DeclaringType) as Controller;
        if (sender is IProtocolLayer layer) {
            Task.Run(async () => {
                IPacket response = null!;
                if (value.GetParameters().Any(p => p.GetCustomAttribute<FromSessionAttribute>() != null)) {
                    response = await (Task<IPacket>)value.Invoke(controller, new object[] { packet, session });
                } else {
                    response = await (Task<IPacket>)value.Invoke(controller, new object[] { packet });
                }
                await layer.SendAsync(session, response);
            }).Wait();
        }
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
}
