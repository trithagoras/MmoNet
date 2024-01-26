
using MmoNet.Core.Network.Protocols;
using System.Net.Sockets;

namespace MmoNet.Core.PlayerSessions; 
public class PlayerSession(Guid id, TcpClient client) : ISession {
    public Guid Id { get; private set; } = id;
    public string Username { get; private set; }
    readonly TcpClient client = client;

    public async Task StartAsync() {
        
    }
}
