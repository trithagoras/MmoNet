
using MmoNet.Core.Network.Packets;
using MmoNet.Core.PlayerSessions;

namespace MmoNet.Core.Network.Protocols; 
public interface IProtocolLayer {
    public Task StartAsync(int port);
    public Task StopAsync();
    public Task SendAsync(ISession session, IPacket packet);
    public event EventHandler<IPacket> OnPacketReceived;
    public event EventHandler<IPacket> OnPacketSent;
    public event EventHandler<ISession> OnConnected;
    public event EventHandler<ISession> OnDisconnected;
}
