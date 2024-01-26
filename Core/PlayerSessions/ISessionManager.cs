
namespace MmoNet.Core.PlayerSessions; 
public interface ISessionManager {
    public Dictionary<Guid, ISession> SessionMap { get; }
    //public ISession GetSession(Guid id);
    //public void AddSession(ISession session);
    //public void RemoveSession(Guid id);
}
