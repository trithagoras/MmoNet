using MmoNet.Core.PlayerSessions;

namespace Sample; 
public class SessionGuid : ISessionId {
    public Guid Id { get; } = Guid.NewGuid();
}
