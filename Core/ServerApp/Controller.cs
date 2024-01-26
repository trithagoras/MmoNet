

using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.ServerApp; 
public abstract class Controller {
    protected IPacket Ok(object? response) {
        return new OkPacket() {
            Result = response
        };
    }

    protected IPacket Deny(object? response) {
        return new DenyPacket() {
            Result = response
        };
    }
}
