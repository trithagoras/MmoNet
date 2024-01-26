
namespace MmoNet.Core.PlayerSessions; 
public interface ISessionManager {
    public Dictionary<ISessionId, IPlayerSession> SessionMap { get; }
}
