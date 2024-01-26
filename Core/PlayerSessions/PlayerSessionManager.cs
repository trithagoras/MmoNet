

namespace MmoNet.Core.PlayerSessions;
public class PlayerSessionManager : ISessionManager {
    public Dictionary<Guid, ISession> SessionMap { get; } = [];
}
