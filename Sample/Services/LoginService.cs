

using MmoNet.Core.Sessions;
using Sample.Exceptions;
using Sample.States;

namespace Sample.Services;
public class LoginService : ILoginService {
    public async Task LoginAsync(PlayerSession session, string username, string password) {
        if (username != "jon") {
            throw new EntryException("Invalid username");
        }
        session.SetState<PlayState>();
    }

    public async Task LogoutAsync(PlayerSession session, string username) {
        session.SetState<EntryState>();
        return;
    }
}
