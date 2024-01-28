using MmoNet.Shared.Packets;

namespace MmoNet.Shared.Serializers; 
public interface ISerializer {
    public byte[] Serialize(IPacket packet);
    public IPacket Deserialize(byte[] bytes);
}
