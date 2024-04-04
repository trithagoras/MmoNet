using MmoNet.Shared.Packets;

namespace Sample.Packets;
[PacketId(1000)]
public class EchoPacket : Packet {
    public required string Message { get; set; }
}
