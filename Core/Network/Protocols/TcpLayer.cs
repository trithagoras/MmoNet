using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using System.Net;
using System.Net.Sockets;
using MmoNet.Core.ServerApp.Exceptions;

namespace MmoNet.Core.Network.Protocols; 
public class TcpLayer(ISessionManager sessionManager, ISerializer serializer, ILogger<TcpLayer> logger) : IProtocolLayer {
    TcpListener listener = null!;
    readonly ISerializer serializer = serializer;
    readonly ILogger<TcpLayer> logger = logger;
    readonly ISessionManager sessionManager = sessionManager;
    public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;

    public event EventHandler<IPacket>? OnPacketReceived;
    public event EventHandler<IPacket>? OnPacketSent;
    public event EventHandler<ISession>? OnConnected;
    public event EventHandler<ISession>? OnDisconnected;
    public event EventHandler<ActionExceptionContext>? OnException;

    readonly Dictionary<Guid, TcpClient> sessionClientMap = [];

    public async Task SendAsync(ISession session, IPacket packet) {
        var client = sessionClientMap[session.Id];
        var stream = client.GetStream();
        var bytes = serializer.Serialize(packet);
        await stream.WriteAsync(bytes);
        await stream.FlushAsync();
        OnPacketSent?.Invoke(this, packet);
    }

    public async Task BroadcastAsync(ISession session, IPacket packet) {
        var bytes = serializer.Serialize(packet);
        foreach (var client in sessionClientMap.Values) {
            if (client == sessionClientMap[session.Id]) {
                continue;
            }
            var stream = client.GetStream();
            await stream.WriteAsync(bytes);
            await stream.FlushAsync();
        }
        logger.LogInformation("Broadcasted packet {packet} to {count} sessions.", packet, sessionClientMap.Count - 1);
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
            _ = StartClientAsync(client, session);
        }
    }

    public async Task StopAsync() {
        listener.Stop();
        listener.Dispose();
    }

    async Task StartClientAsync(TcpClient client, ISession session) {
        var stream = client.GetStream();

        while (client.Connected) {
            try {
                var bytes = new byte[1024];
                var length = await stream.ReadAsync(bytes);
                if (length == 0) {
                    break;
                }
                var packet = serializer.Deserialize(bytes[..length]);
                OnPacketReceived?.Invoke(this, packet);
            } catch (Exception e) {
                HandleClientError(e, client, session);
                return;
            }
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
        if (e is SocketException || e is IOException) {
            StopClient(client, session);
            return;
        }
        var exceptionContext = new ActionExceptionContext(session, null, e);
        OnException?.Invoke(this, exceptionContext);
    }
}
