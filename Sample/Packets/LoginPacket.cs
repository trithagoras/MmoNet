using MmoNet.Shared.Packets;

namespace Sample.Packets;
[PacketId(4)]
public class LoginPacket : Packet {
    public required string Username { get; set; }
    public required string Password { get; set; }
}
