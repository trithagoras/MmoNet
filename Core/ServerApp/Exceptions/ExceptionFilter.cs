using Microsoft.Extensions.Logging;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.ServerApp.Exceptions;
/// <summary>
/// A base class for exception filters. It sends a DenyPacket to the client when an InvalidStateException is caught or a generic error message otherwise.
/// </summary>
public abstract class ExceptionFilter(IProtocolLayer protocol) : IExceptionFilter {
    readonly IProtocolLayer protocol = protocol;

    public virtual void OnException(ActionExceptionContext ctx) {
        switch (ctx.Exception) {
            case InvalidStateException invalidStateException:
                protocol.SendAsync(ctx.Session, new DenyPacket {
                    SessionId = ctx.Session.Id,
                    Result = invalidStateException.Message
                });
                break;
            default:
                protocol.SendAsync(ctx.Session, new DenyPacket {
                    SessionId = ctx.Session.Id,
                    Result = "An internal error occurred."
                });
                break;
        }
    }
}
