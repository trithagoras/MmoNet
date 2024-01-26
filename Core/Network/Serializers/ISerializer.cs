
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.Network.Serializers; 
public interface ISerializer {
    public byte[] Serialize(IPacket packet);
    public IPacket Deserialize(byte[] bytes);
}
