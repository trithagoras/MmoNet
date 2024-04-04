
using MmoNet.Core.Sessions;

namespace Sample.Services; 
public interface ILoginService {
    public Task LoginAsync(PlayerSession session, string username, string password);
    public Task LogoutAsync(PlayerSession session, string username);
}
