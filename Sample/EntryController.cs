using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using MmoNet.Core.States;
using Sample.Packets;
using Sample.Services;

namespace Sample; 
public class EntryController(ILoginService service, ILogger<EntryController> logger) : Controller {
    readonly ILoginService service = service;
    readonly ILogger<EntryController> logger = logger;

    public async Task<IPacket> Login(LoginPacket packet, [FromSession] ISession session) {
        await service.LoginAsync(packet.Username, packet.Password);
        return Ok(session, "logged in successfully");
    }

    public async Task<IPacket> Logout(LogoutPacket _, [FromSession] ISession session) {
        await service.LogoutAsync("jon");
        return Ok(session, "logged out successfully");
    }
}
