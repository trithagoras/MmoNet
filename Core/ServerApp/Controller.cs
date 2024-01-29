using MmoNet.Shared.Packets;
using MmoNet.Core.Sessions;

namespace MmoNet.Core.ServerApp; 
public abstract class Controller {
    protected IPacket Ok(ISession session, object? response = null) {
        return new OkPacket() {
            SessionId = session.Id,
            Result = response
        };
    }

    protected IPacket Deny(ISession session, object? response = null) {
        return new DenyPacket() {
            SessionId = session.Id,
            Result = response
        };
    }
}
