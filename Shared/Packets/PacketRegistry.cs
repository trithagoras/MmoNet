using MmoNet.Shared.Packets;

namespace MmoNet.Shared.Packets;
public class PacketRegistry : IPacketRegistry {
    private IDictionary<int, Type> packetTypes = new Dictionary<int, Type>();

    public void RegisterPacket<T>() where T : IPacket {
        var packet = Activator.CreateInstance<T>();
        packetTypes.Add(packet.PacketId, typeof(T));
    }

    public void RegisterPackets(IDictionary<int, Type> map) {
        packetTypes = map;
    }

    public int GetPacketId<T>() where T : IPacket {
        return packetTypes.FirstOrDefault(x => x.Value == typeof(T)).Key;
    }

    public Type GetPacketType(int packetId) {
        return packetTypes[packetId];
    }
}
