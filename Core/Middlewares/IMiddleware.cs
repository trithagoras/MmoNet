
using MmoNet.Shared.Packets;

namespace MmoNet.Core.Middlewares;
public interface IMiddleware {
    public Task InvokeAsync(IPacket request);
}
