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
        var packet = new OkPacket();
        var bytes = serializer.Serialize(packet);
        var s = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(json, s);
    }

    [TestMethod]
    public void TestDeserializePacket() {
        var bytes = Encoding.UTF8.GetBytes(json);
        var packet = serializer.Deserialize(bytes);
        Assert.AreEqual(PacketIdsExtension.OkPacket, packet.PacketId);
        Assert.AreEqual(typeof(OkPacket), packet.GetType());
        Assert.AreEqual(packet.SessionId, new Guid());
        Assert.IsNull((packet as OkPacket)!.Result);
    }
}
