namespace MmoNet.Core.Network.Packets;
public class DenyPacket : Packet {
    public override int PacketId => PacketIds.DenyPacket;
    public object? Result { get; set; }
}
