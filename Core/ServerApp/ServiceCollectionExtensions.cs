using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.ServerApp;
public static class ServiceCollectionExtensions {
    public static ServiceCollection AddProtocolLayer<T>(this ServiceCollection services) where T : class, IProtocolLayer {
        services.AddSingleton<IProtocolLayer, T>();
        return services;
    }

    public static ServiceCollection AddProtocolLayer(this ServiceCollection services, IProtocolLayer protocolLayer) {
        services.AddSingleton(protocolLayer);
        return services;
    }

    public static ServiceCollection AddSessionManager<T>(this ServiceCollection services) where T : class, ISessionManager {
        services.AddSingleton<ISessionManager, T>();
        return services;
    }

    public static ServiceCollection AddSessionManager(this ServiceCollection services, ISessionManager sessionManager) {
        services.AddSingleton(sessionManager);
        return services;
    }

    public static ServiceCollection AddSerializer<T>(this ServiceCollection services) where T : class, ISerializer {
        services.AddSingleton<ISerializer, T>();
        return services;
    }

    public static ServiceCollection AddSerializer(this ServiceCollection services, ISerializer serializer) {
        services.AddSingleton(serializer);
        return services;
    }

    public static ServiceCollection AddControllers(this ServiceCollection services) {
        var controllers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Controller)))
            .ToList();
        controllers?.ForEach(c => services.AddSingleton(c));
        return services;
    }

    public static ServiceCollection AddPacketRegistry<T>(this ServiceCollection services) where T : class, IPacketRegistry {
        services.AddSingleton<IPacketRegistry, T>();
        return services;
    }
}
