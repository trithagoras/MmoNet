using MmoNet.Core.Network.Packets;

namespace Sample.Packets; 
public class LogoutPacket : Packet {
    public override int PacketId => PacketIdsExtension.LogoutPacket;
}
