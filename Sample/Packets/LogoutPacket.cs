using MmoNet.Shared.Packets;

namespace Sample.Packets;
[PacketId(((int)PacketIds.LogoutPacket))]
public class LogoutPacket : Packet {
}
