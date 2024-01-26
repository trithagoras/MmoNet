
namespace MmoNet.Core.Network.Packets; 
public interface IPacket {
    public int PacketId { get; }
    public Guid SessionId { get; set; }
}
