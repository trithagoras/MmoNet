
namespace MmoNet.Core.ServerApp;
public class ServerEngine(int tickRate) : IServerEngine {

    float deltaTime = 0;

    public int TickRate { get; } = tickRate;

    public float DeltaTime => deltaTime;

    public event EventHandler OnTick = null!;

    public void Tick(float deltaTime) {
        this.deltaTime = deltaTime;
        OnTick.Invoke(null, EventArgs.Empty);
    }
}
