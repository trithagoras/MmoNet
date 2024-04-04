
using Microsoft.Extensions.Logging;
using MmoNet.Core.ServerApp;
using MmoNet.Core.Sessions;
using MmoNet.Core.States;
using MmoNet.Shared.Packets;
using Sample.Packets;
using Sample.Services;
using Sample.States;

namespace Sample.Controllers;
public class PlayController(ILoginService service, ILogger<EntryController> logger) : Controller {
    readonly ILoginService service = service;
    readonly ILogger<EntryController> logger = logger;
    
    [RequiresState<PlayState>]
    public async Task<IPacket> Logout(LogoutPacket _, [FromSession] PlayerSession session) {
        await service.LogoutAsync(session, "jon");
        return Ok(session, "logged out successfully");
    }

    // Example of a controller action that can be matched regardless of the session's state
    public async Task<IPacket> Echo(EchoPacket packet) {
        return packet;
    }
}
