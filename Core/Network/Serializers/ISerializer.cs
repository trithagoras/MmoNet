
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.Network.Serializers; 
public interface ISerializer {
    public Task<byte[]> Serialize(IPacket packet);
    public Task<IPacket> Deserialize(byte[] bytes);
}
