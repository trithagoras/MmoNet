

using MmoNet.Core.States;

namespace MmoNet.Core.Sessions; 
public interface ISession {
    Guid Id { get; }
    ISessionState State { get; }
}
