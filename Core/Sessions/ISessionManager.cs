
namespace MmoNet.Core.Sessions; 
public interface ISessionManager {
    IEnumerable<ISession> Sessions { get; }
    ISession this[Guid id] { get; set; }
    /// <summary>
    /// Creates a new session and returns it. The session should not be automatically added to the session manager
    /// and should be added manually
    /// </summary>
    /// <returns></returns>
    ISession CreateSession();
    void RemoveSession(Guid id);
}
