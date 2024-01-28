using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.Sessions;
using System.Reflection;
using System;
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.ServerApp;
public class ServerBuilder {
    public ServiceCollection Services { get; }

    public ServerBuilder() {
        Services = new();
    }

    public (ServerApplication, ServiceProvider) Build() {
        Services.AddLogging(o => o.AddConsole());
        Services.AddPacketRegistry<PacketRegistry>();
        Services.AddControllers();

        var serviceProvider = Services.BuildServiceProvider();
        var app = ActivatorUtilities.CreateInstance<ServerApplication>(serviceProvider, serviceProvider);
        app.MapControllers();
        app.RegisterPackets();
        return (app, serviceProvider);
    }
}