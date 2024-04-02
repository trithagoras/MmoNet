
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.ServerApp.Exceptions;
using MmoNet.Shared.Packets;

namespace Sample.Exceptions;
public class ExceptionFilter(IProtocolLayer protocol) : IExceptionFilter {
    readonly IProtocolLayer protocol = protocol;
    public void OnException(ActionExceptionContext ctx) {
        if (ctx.Exception is EntryException) {
            protocol.SendAsync(ctx.Session, new DenyPacket() {
                SessionId = ctx.Session.Id,
                Result = ctx.Exception.Message
            });
        }
    }
}
