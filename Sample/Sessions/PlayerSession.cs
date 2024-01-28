
using MmoNet.Core.Network.Protocols;
using System.Net.Sockets;

namespace MmoNet.Core.Sessions; 
public class PlayerSession : ISession {
    public Guid Id { get; } = Guid.NewGuid();
}
