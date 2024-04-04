

using MmoNet.Core.States;

namespace MmoNet.Core.Sessions; 
public interface ISession {
    public Guid Id { get; }
    public ISessionState State { get; }
}
