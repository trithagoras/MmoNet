using System.Collections.Concurrent;

namespace MmoNet.Core.Sessions;
public class PlayerSessionManager : ISessionManager {
    readonly ConcurrentDictionary<Guid, ISession> sessions = new();

    public ISession this[Guid id] { 
        get => sessions[id];
        set => sessions[id] = value;
    }

    public ISession CreateSession() {
        var session = new PlayerSession();
        return session;
    }

    public void RemoveSession(Guid id) {
        if (!sessions.TryRemove(id, out _)) {
            throw new InvalidOperationException($"Session with id {id} does not exist");
        }
    }
}
