using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Middlewares;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;

namespace MmoNet.Core.ServerApp; 
public class ServerApplication(IProtocolLayer protocolLayer, ISerializer serializer, ISessionManager sessionManager) {

    readonly List<IMiddleware> middlewares = [];

    public async Task StartAsync(int port) {
        await protocolLayer.StartAsync(port);
    }

    public void AddMiddleware<T>() where T : IMiddleware, new() {
        // Check if a middleware of type T already exists in the list
        if (middlewares.Any(m => m.GetType() == typeof(T))) {
            throw new InvalidOperationException($"Middleware of type {typeof(T).Name} is already added.");
        }
        middlewares.Add(new T());
    }
}
