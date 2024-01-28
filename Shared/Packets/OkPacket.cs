namespace MmoNet.Shared.Packets;
[PacketId(2)]
public class OkPacket : Packet {
    public object? Result { get; set; }
}
