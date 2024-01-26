
namespace MmoNet.Core.PlayerSessions; 
public class PlayerSession(Guid id) : IPlayerSession {
    public Guid Id { get; private set; } = id;
    public string Username { get; private set; }
}
