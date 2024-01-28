using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using System.Net;
using System.Net.Sockets;

namespace MmoNet.Core.Network.Protocols; 
public class TcpLayer(ISessionManager sessionManager, ISerializer serializer, ILogger<TcpLayer> logger) : IProtocolLayer {
    TcpListener listener = null!;
    readonly ISerializer serializer = serializer;
    readonly ILogger<TcpLayer> logger = logger;
    readonly ISessionManager sessionManager = sessionManager;
    public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;

    public event EventHandler<IPacket> OnPacketReceived;
    public event EventHandler<IPacket> OnPacketSent;
    public event EventHandler<ISession> OnConnected;
    public event EventHandler<ISession> OnDisconnected;

    readonly Dictionary<Guid, TcpClient> sessionClientMap = [];

    public async Task SendAsync(ISession session, IPacket packet) {
        var client = sessionClientMap[session.Id];
        var stream = client.GetStream();
        var bytes = serializer.Serialize(packet);
        await stream.WriteAsync(bytes);
        await stream.FlushAsync();
        OnPacketSent?.Invoke(this, packet);
    }

    public async Task StartAsync(int port) {
        listener = new(IPAddress.Any, port);
        listener.Start();
        logger.LogInformation("Server listening on port {port}", Port);
        while (true) {
            var client = await listener.AcceptTcpClientAsync();
            var session = sessionManager.CreateSession();
            sessionClientMap.Add(session.Id, client);
            OnConnected?.Invoke(this, session);
            _ = StartClientAsync(client, session)
                .ContinueWith(t => {
                    HandleClientError(t.Exception!, client, session);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public async Task StopAsync() {
        listener.Stop();
        listener.Dispose();
    }

    async Task StartClientAsync(TcpClient client, ISession session) {
        var stream = client.GetStream();

        while (true) {
            var bytes = new byte[1024];
            var length = await stream.ReadAsync(bytes);
            if (length == 0) {
                break;
            }
            var packet = serializer.Deserialize(bytes[..length]);
            OnPacketReceived?.Invoke(this, packet);
        }

        StopClient(client, session);
    }

    void StopClient(TcpClient client, ISession session) {
        OnDisconnected?.Invoke(this, session);
        sessionClientMap.Remove(session.Id);
        client.Close();
    }

    void HandleClientError(Exception e, TcpClient client, ISession session) {
        logger.LogError(e, "Error in StartClientAsync from {sessionId}", session.Id);
        StopClient(client, session);
    }
}
