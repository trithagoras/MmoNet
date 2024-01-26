
namespace MmoNet.Core.Network.Protocols; 
public interface IProtocolLayer {
    public Task StartAsync(int port);
    public Task StopAsync();
}
