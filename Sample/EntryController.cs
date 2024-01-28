using Microsoft.Extensions.Logging;
using MmoNet.Core.Network.Packets;
using MmoNet.Core.PlayerSessions;
using MmoNet.Core.ServerApp;
using MmoNet.Core.States;
using Sample.Packets;
using Sample.Services;

namespace Sample; 
public class EntryController(ILoginService service, ILogger<EntryController> logger, ISessionManager sessionManager) : Controller {
    readonly ILoginService service = service;
    readonly ILogger<EntryController> logger = logger;
    readonly ISessionManager sessionManager = sessionManager;

    [RequiresState(State.Entry)]
    public async Task<IPacket> Login(LoginPacket packet) {
        var result = await service.LoginAsync(packet.Username, packet.Password);
        var session = sessionManager.SessionMap[packet.SessionId];
        return Ok(result, session);
    }

    [RequiresState(State.Any, State.Entry)]
    public async Task<IPacket> Logout(LogoutPacket packet) {
        var result = await service.LogoutAsync("");
        var session = sessionManager.SessionMap[packet.SessionId];
        return Ok(result, session);
    }

    public async Task<IPacket> TestOkPacket(OkPacket packet) {
        var session = sessionManager.SessionMap[packet.SessionId];
        logger.LogInformation("{sessionId} received an OkPacket", session.Id);
        return Ok(packet.Result, session);
    }
}
