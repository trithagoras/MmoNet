using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MmoNet.Core.Middlewares;
using MmoNet.Core.Network.Packets;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;

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
        await protocolLayer.StartAsync(port);
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
        var controllers = Assembly.GetEntryAssembly()?.GetTypes()
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
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(nameof(IPacket)) != null))
            .ToList();

        packetTypes.ForEach(t => {
            var instance = Activator.CreateInstance(t) as IPacket;
            var packetId = instance.PacketId;
            map.Add(packetId, t);
        });

        packetRegistry.RegisterPackets(map);
    }

    void DispatchPacket(object? sender, IPacket packet) {
        logger.BeginScope("SessionId: {sessionId}", packet.SessionId);

        var session = sessionManager.SessionMap.Values.FirstOrDefault(s => s.Id == packet.SessionId);
        if (session == null) {
            // log warning
            logger.LogWarning("Received packet with invalid session id.");
            return;
        }

        if (!ActionMap.TryGetValue(packet.PacketId, out MethodInfo? value)) {
            logger.LogWarning("Received packet with invalid packet id.");
            // log warning
            return;
        }

        var controller = serviceProvider.GetRequiredService(value.DeclaringType) as Controller;
        value.Invoke(controller, new object[] { packet });
    }
}
