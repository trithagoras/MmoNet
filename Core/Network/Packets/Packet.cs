namespace MmoNet.Core.Network.Packets;
public abstract class Packet : IPacket {
    public abstract int PacketId { get; }
    public Guid SessionId { get; set; }
}
