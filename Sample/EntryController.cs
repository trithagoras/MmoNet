
using MmoNet.Core.ServerApp;
using MmoNet.Core.States;
using Sample.Packets;
using Sample.Services;

namespace Sample; 
public class EntryController(ILoginService service) : Controller {
    readonly ILoginService service = service;

    [RequiresState(State.Entry)]
    public async Task<IMmoResponse> Login(LoginPacket packet) {
        var result = await service.LoginAsync(packet.Username, packet.Password);
        return Ok(result);
    }

    [RequiresState(State.Any, State.Entry)]
    public async Task<IMmoResponse> Logout() {
        var result = await service.LogoutAsync("");
        return Ok(result);
    }
}
