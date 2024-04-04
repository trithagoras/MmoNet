using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using MmoNet.Core.States;
using Sample.Packets;
using Sample.Services;
using Sample.States;

namespace Sample.Controllers;
public class EntryController(ILoginService service, ILogger<EntryController> logger) : Controller {
    readonly ILoginService service = service;
    readonly ILogger<EntryController> logger = logger;

    [RequiresState<EntryState>()]
    public async Task<IPacket> Login(LoginPacket packet, [FromSession] PlayerSession session) {
        await service.LoginAsync(session, packet.Username, packet.Password);
        return Ok(session, "logged in successfully");
    }
}
