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
        return Ok(result);
    }

    [RequiresState(State.Any, State.Entry)]
    public async Task<IPacket> Logout(LogoutPacket packet) {
        var result = await service.LogoutAsync("");
        return Ok(result);
    }

    public async Task<IPacket> TestOkPacket(OkPacket packet) {
        var session = sessionManager.SessionMap[packet.SessionId];
        logger.LogInformation("{sessionId} received an OkPacket", session.Id);
        return Ok(packet.Result);
    }
}
