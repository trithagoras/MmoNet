using Microsoft.Extensions.Logging;
using MmoNet.Core.ServerApp;
using MmoNet.Core.Sessions;
using Sample.Exceptions;
using Sample.States;

namespace Sample.Services;
public class LoginService : ILoginService {

    private readonly ILogger<LoginService> logger;
    private readonly IServerEngine engine;

    public LoginService(ILogger<LoginService> logger, IServerEngine engine) {
        this.logger = logger;
        this.engine = engine;
        engine.OnTick += OnTick;
    }

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

    private void OnTick(object? sender, EventArgs e) {
        logger.LogDebug("delta: {dt}", engine.DeltaTime);
    }
}
