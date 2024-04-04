
using MmoNet.Core.ServerApp.Exceptions;
using MmoNet.Core.Sessions;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.Network.Protocols; 
public interface IProtocolLayer {
    public Task StartAsync(int port);
    public Task StopAsync();
    public Task SendAsync(ISession session, IPacket packet);
    public Task BroadcastAsync(ISession session, IPacket packet);
    public event EventHandler<IPacket> OnPacketReceived;
    public event EventHandler<IPacket> OnPacketSent;
    public event EventHandler<ISession> OnConnected;
    public event EventHandler<ISession> OnDisconnected;
    public event EventHandler<ActionExceptionContext> OnException;
}
