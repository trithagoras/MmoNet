

namespace MmoNet.Core.PlayerSessions; 
public interface ISession {
    public Guid Id { get; }
    public Task StartAsync();
}
