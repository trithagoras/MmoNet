namespace MmoNet.Shared.Packets;
[PacketId(3)]
public class DenyPacket : Packet {
    public object? Result { get; set; }
}
