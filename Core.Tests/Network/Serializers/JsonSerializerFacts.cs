using MmoNet.Core.Network.Packets;
using MmoNet.Core.Network.Serializers;
using Sample.Packets;
using System.Text;

namespace MmoNet.Core.Tests.Network.Serializers;
[TestClass]
public class JsonSerializerFacts {
    ISerializer serializer = null!;
    readonly string json = $$"""{"PacketId":{{PacketIdsExtension.OkPacket}},"SessionId":"00000000-0000-0000-0000-000000000000"}""";

    [TestInitialize]
    public void SetUp() {
        var packetRegistry = new PacketRegistry();
        packetRegistry.RegisterPacket<OkPacket>();
        packetRegistry.RegisterPacket<LoginPacket>();
        serializer = new JsonSerializer(packetRegistry);
    }

    [TestMethod]
    public void TestSerializePacket() {
    }

    [TestMethod]
    public void TestDeserializePacket() {
        var bytes = Encoding.UTF8.GetBytes(json);
        var packet = serializer.Deserialize(bytes);
        Assert.AreEqual(PacketIdsExtension.OkPacket, packet.PacketId);
    }
}
