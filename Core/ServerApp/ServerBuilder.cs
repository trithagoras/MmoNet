using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;

namespace MmoNet.Core.ServerApp;
public class ServerBuilder {
    public ServiceCollection Services { get; }

    public ServerBuilder() {
        Services = new();
    }

    public ServerApplication Build() {
        var serviceProvider = Services.BuildServiceProvider();

        // initialize core components with the service provider
        var protocolLayer = serviceProvider.GetRequiredService<IProtocolLayer>();
        var sessionManager = serviceProvider.GetRequiredService<ISessionManager>();
        var serializer = serviceProvider.GetRequiredService<ISerializer>();

        var serverApplication = new ServerApplication(protocolLayer, serializer, sessionManager);

        // ...

        return serverApplication;
    }
}