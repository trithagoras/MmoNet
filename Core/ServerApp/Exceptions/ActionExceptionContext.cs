using MmoNet.Core.Sessions;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.ServerApp.Exceptions; 
public class ActionExceptionContext(ISession session, IPacket packet, Exception exception) {
    public ISession Session { get; } = session;
    public IPacket? Packet { get; } = packet;
    public Exception Exception { get; } = exception;
}
