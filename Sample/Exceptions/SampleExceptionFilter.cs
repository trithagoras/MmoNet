
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.ServerApp.Exceptions;
using MmoNet.Shared.Packets;

namespace Sample.Exceptions;
public class SampleExceptionFilter(IProtocolLayer protocol) : ExceptionFilter(protocol) {
    readonly IProtocolLayer protocol = protocol;
    public override void OnException(ActionExceptionContext ctx) {
        switch (ctx.Exception) {
            case EntryException entryException:
                protocol.SendAsync(ctx.Session, new DenyPacket() {
                    SessionId = ctx.Session.Id,
                    Result = entryException.Message
                });
                break;
        }
        base.OnException(ctx);
    }
}
