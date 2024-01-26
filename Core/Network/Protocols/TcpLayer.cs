using Microsoft.Extensions.Logging;
using MmoNet.Core.Network.Packets;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;
using System.Net;
using System.Net.Sockets;

namespace MmoNet.Core.Network.Protocols; 
public class TcpLayer(ISessionManager sessionManager, ISerializer serializer, ILogger<TcpLayer> logger) : IProtocolLayer {
    TcpListener listener = null!;
    readonly ISerializer serializer = serializer;
    readonly ILogger<TcpLayer> logger = logger;

    public event EventHandler<IPacket> OnPacketReceived;
    public event EventHandler<IPacket> OnPacketSent;
    public event EventHandler<ISession> OnConnected;

    public async Task SendAsync(ISession session, IPacket packet) {
        //var client = session.Client;
        //var stream = client.GetStream();
        //var bytes = await packet.Serialize();
        //await stream.WriteAsync(bytes);
        OnPacketSent?.Invoke(this, packet);
    }

    public async Task StartAsync(int port) {
        listener = new(IPAddress.Any, port);
        listener.Start();
        logger.LogInformation("Server listening on port {port}", ((IPEndPoint)listener.LocalEndpoint).Port);
        while (true) {
            var client = await listener.AcceptTcpClientAsync();
            var session = new PlayerSession(Guid.NewGuid(), client);
            OnConnected?.Invoke(this, session);
            sessionManager.SessionMap.Add(session.Id, session);
            StartClientAsync(client, session);
        }
    }

    public async Task StartClientAsync(TcpClient client, ISession session) {
        var stream = client.GetStream();
        logger.LogInformation("Client {sessionId} has connected.", session.Id);

        while (true) {
            var bytes = new byte[1024];
            var length = await stream.ReadAsync(bytes);
            if (length == 0) {
                break;
            }
            var packet = serializer.Deserialize(bytes[..length]);
            OnPacketReceived?.Invoke(this, packet);
        }

        sessionManager.SessionMap.Remove(session.Id);
        logger.LogInformation("Client {sessionId} has disconnected.", session.Id);
    }



    public async Task StopAsync() {
        listener.Stop();
        listener.Dispose();
    }
}
