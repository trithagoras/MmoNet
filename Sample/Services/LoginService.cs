

using Sample.Exceptions;

namespace Sample.Services;
public class LoginService : ILoginService {
    public async Task LoginAsync(string username, string password) {
        if (username == "jon") {
            return;
        }
        throw new EntryException("Invalid username");
    }

    public async Task LogoutAsync(string username) {
        return;
    }
}
