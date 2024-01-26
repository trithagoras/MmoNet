
using MmoNet.Core.Network.Packets;

namespace MmoNet.Core.ServerApp; 
public class MmoResponse(object response) : IMmoResponse {
    public object Response { get; set; } = response;
}
