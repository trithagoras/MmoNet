

namespace Sample.Services;
public class LoginService : ILoginService {
    public async Task<bool> LoginAsync(string username, string password) {
        return true;
    }

    public async Task<bool> LogoutAsync(string username) {
        return true;
    }
}
