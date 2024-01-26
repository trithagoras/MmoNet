
namespace MmoNet.Core.Network.Packets;
public interface IPacketRegistry {
    void RegisterPackets(IDictionary<int, Type> map);
    void RegisterPacket<T>() where T : IPacket;
    public int GetPacketId<T>() where T : IPacket;
    public Type GetPacketType(int packetId);
}
