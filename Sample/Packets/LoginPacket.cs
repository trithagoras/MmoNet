
using MmoNet.Core.Network.Packets;

namespace Sample.Packets; 
public class LoginPacket : Packet {
    public override int PacketId => PacketIdsExtension.LoginPacket;
    public required string Username { get; set; }
    public required string Password { get; set; }
}
