using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.Tests.Network.Packets;
public class TestPacket : Packet {
    public override int PacketId => int.MaxValue;
}
