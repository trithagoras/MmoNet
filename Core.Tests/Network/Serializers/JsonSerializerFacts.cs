using MmoNet.Shared.Packets;
using MmoNet.Shared.Serializers;
using Sample.Packets;
using System.Text;

namespace MmoNet.Core.Tests.Network.Serializers;
[TestClass]
public class JsonSerializerFacts {
    ISerializer serializer = null!;
    IPacketRegistry registry = null!;
    string Json => $$"""{"packetId":{{registry.GetPacketId<OkPacket>()}},"sessionId":"00000000-0000-0000-0000-000000000000"}""";

    [TestInitialize]
    public void SetUp() {
        registry = new PacketRegistry();
        registry.RegisterPacket<OkPacket>();
        registry.RegisterPacket<LoginPacket>();
        serializer = new JsonSerializer(registry);
    }

    [TestMethod]
    public void TestSerializePacket() {
        var packet = new OkPacket();
        var bytes = serializer.Serialize(packet);
        var s = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(Json, s);
    }

    [TestMethod]
    public void TestDeserializePacket() {
        var bytes = Encoding.UTF8.GetBytes(Json);
        var packet = serializer.Deserialize(bytes);
        Assert.AreEqual(registry.GetPacketId<OkPacket>(), packet.PacketId);
        Assert.AreEqual(typeof(OkPacket), packet.GetType());
        Assert.AreEqual(packet.SessionId, new Guid());
        Assert.IsNull((packet as OkPacket)!.Result);
    }
}
