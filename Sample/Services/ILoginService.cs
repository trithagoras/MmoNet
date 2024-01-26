
namespace Sample.Services; 
public interface ILoginService {
    public Task<bool> LoginAsync(string username, string password);
    public Task<bool> LogoutAsync(string username);
}
