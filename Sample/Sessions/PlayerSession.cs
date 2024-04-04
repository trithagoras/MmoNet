
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.States;
using Sample.States;
using System.Net.Sockets;

namespace MmoNet.Core.Sessions; 
public class PlayerSession : ISession {
    public Guid Id { get; } = Guid.NewGuid();
    public ISessionState State { get; private set; } = new EntryState();

    public void SetState<T>() where T : ISessionState, new() {
        State = new T();
    }
}
