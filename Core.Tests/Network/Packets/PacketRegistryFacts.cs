
using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Packets;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;
using MmoNet.Core.ServerApp;
using Sample.Services;
using System.Reflection;

namespace MmoNet.Core.Tests.Network.Packets;
[TestClass]
public class PacketRegistryFacts {

    ServerApplication app;
    IServiceProvider provider;

    [TestInitialize]
    public void SetUp() {
        // building sets up automatic packet registration
        var builder = new ServerBuilder();
        builder.Services.AddProtocolLayer(Substitute.For<IProtocolLayer>());
        builder.Services.AddSessionManager(Substitute.For<ISessionManager>());
        builder.Services.AddSerializer(Substitute.For<ISerializer>());
        var (app, provider) = builder.Build();
        this.app = app;
        this.provider = provider;
    }

    [TestMethod]
    public void TestPacketAutomaticRegistration() {
        var registry = provider.GetRequiredService<IPacketRegistry>();
        try {
            registry.GetPacketType(PacketIdsExtension.TestPacketId);
            registry.GetPacketType(PacketIdsExtension.OkPacket);
            registry.GetPacketType(PacketIdsExtension.DenyPacket);
            registry.GetPacketType(PacketIdsExtension.IdPacket);
        } catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }
}
