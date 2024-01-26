
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.Network.Serializers;
public class JsonSerializer : ISerializer {
    public Task<IPacket> Deserialize(byte[] bytes) {
        throw new NotImplementedException();
    }

    public Task<byte[]> Serialize(IPacket packet) {
        throw new NotImplementedException();
    }
}
