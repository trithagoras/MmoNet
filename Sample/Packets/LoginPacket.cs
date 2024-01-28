using MmoNet.Shared.Packets;

namespace Sample.Packets;
[PacketId(((int)PacketIds.LoginPacket))]
public class LoginPacket : Packet {
    public required string Username { get; set; }
    public required string Password { get; set; }
}
