
namespace MmoNet.Core.Sessions; 
public interface ISessionManager {
    public ISession this[Guid id] { get; set; }
    /// <summary>
    /// Creates a new session and returns it. The session should not be automatically added to the session manager
    /// and should be added manually
    /// </summary>
    /// <returns></returns>
    public ISession CreateSession();
    public void RemoveSession(Guid id);
}
