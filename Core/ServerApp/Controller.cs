

using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.ServerApp; 
public abstract class Controller {
    public IMmoResponse Ok(object response) {
        return new MmoResponse(response);
    }
}
