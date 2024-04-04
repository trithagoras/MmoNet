
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.ServerApp.Exceptions;
using MmoNet.Shared.Packets;

namespace Sample.Exceptions;
public class ExceptionFilter(IProtocolLayer protocol) : IExceptionFilter {
    readonly IProtocolLayer protocol = protocol;
    public void OnException(ActionExceptionContext ctx) {
        switch (ctx.Exception) {
            case EntryException entryException:
                protocol.SendAsync(ctx.Session, new DenyPacket() {
                    SessionId = ctx.Session.Id,
                    Result = entryException.Message
                });
                break;
            case InvalidStateException invalidStateException:
                protocol.SendAsync(ctx.Session, new DenyPacket() {
                    SessionId = ctx.Session.Id,
                    Result = invalidStateException.Message
                });
                break;
            default:
                protocol.SendAsync(ctx.Session, new DenyPacket() {
                    SessionId = ctx.Session.Id,
                    Result = "An internal error occurred."
                });
                break;
        }
    }
}
