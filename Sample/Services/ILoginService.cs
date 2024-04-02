
namespace Sample.Services; 
public interface ILoginService {
    public Task LoginAsync(string username, string password);
    public Task LogoutAsync(string username);
}
