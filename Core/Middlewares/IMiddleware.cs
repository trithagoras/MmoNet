
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.Middlewares;
public interface IMiddleware {
    public Task InvokeAsync(IPacket request);
}
