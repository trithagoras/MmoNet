using MmoNet.Core.PlayerSessions;
using System.Net;
using System.Net.Sockets;

namespace MmoNet.Core.Network.Protocols; 
public class TcpLayer(ISessionManager sessionManager) : IProtocolLayer {
    TcpListener listener = null!;

    public async Task StartAsync(int port) {
        listener = new(IPAddress.Any, port);
        listener.Start();
        while (true) {
            var client = await listener.AcceptTcpClientAsync();
            var guid = Guid.NewGuid();
            sessionManager.SessionMap.Add(guid, new PlayerSession(guid));   // todo: modularize this, maybe have a factory or something
        }
    }

    public async Task StopAsync() {
        listener.Stop();
        listener.Dispose();
    }
}
