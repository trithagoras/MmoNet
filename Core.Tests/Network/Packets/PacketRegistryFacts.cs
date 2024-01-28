
using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.Tests.Network.Packets;
[TestClass]
public class PacketRegistryFacts {

    [TestMethod]
    public void TestPacketAutomaticRegistration() {
        // building sets up automatic packet registration
        var builder = new ServerBuilder();
        builder.Services.AddProtocolLayer(Substitute.For<IProtocolLayer>());
        builder.Services.AddSessionManager(Substitute.For<ISessionManager>());
        builder.Services.AddSerializer(Substitute.For<ISerializer>());
        var (_, provider) = builder.Build();

        var registry = provider.GetRequiredService<IPacketRegistry>();
        try {
            registry.GetPacketType(registry.GetPacketId<TestPacket>());
            registry.GetPacketType(registry.GetPacketId<OkPacket>());
            registry.GetPacketType(registry.GetPacketId<DenyPacket>());
            registry.GetPacketType(registry.GetPacketId<IdPacket>());
        } catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [TestMethod]
    public void TestPacketManualRegistration() {
        var registry = new PacketRegistry();
        registry.RegisterPacket<TestPacket>();
        registry.RegisterPacket<OkPacket>();
        registry.RegisterPacket<DenyPacket>();
        registry.RegisterPacket<IdPacket>();
        try {
            registry.GetPacketType(registry.GetPacketId<TestPacket>());
            registry.GetPacketType(registry.GetPacketId<OkPacket>());
            registry.GetPacketType(registry.GetPacketId<DenyPacket>());
            registry.GetPacketType(registry.GetPacketId<IdPacket>());
        } catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [TestMethod]
    public void TestDuplicatePacketRegistration() {
        var registry = new PacketRegistry();
        registry.RegisterPacket<TestPacket>();
        Assert.ThrowsException<ArgumentException>(() => registry.RegisterPacket<TestPacket>());
    }

    [TestMethod]
    public void TestGetPacketId() {
        var registry = new PacketRegistry();
        registry.RegisterPacket<TestPacket>();
        Assert.AreEqual(5, registry.GetPacketId<TestPacket>());
    }

    [TestMethod]
    public void TestGetPacketType() {
        var registry = new PacketRegistry();
        registry.RegisterPacket<TestPacket>();
        Assert.AreEqual(typeof(TestPacket), registry.GetPacketType(registry.GetPacketId<TestPacket>()));
    }
}
