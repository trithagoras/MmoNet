using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.ServerApp;
public class ServerBuilder {
    public ServiceCollection Services { get; }

    public ServerBuilder() {
        Services = new();
    }

    public (ServerApplication, ServiceProvider) Build() {
        Services.AddControllers();

        var serviceProvider = Services.BuildServiceProvider();
        var app = ActivatorUtilities.CreateInstance<ServerApplication>(serviceProvider, serviceProvider);
        app.RegisterPackets();
        app.MapControllers();
        return (app, serviceProvider);
    }
}