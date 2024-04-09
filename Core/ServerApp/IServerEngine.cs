namespace MmoNet.Core.ServerApp; 
public interface IServerEngine {
    int TickRate { get; }
    float DeltaTime { get; }
    event EventHandler OnTick;
    void Tick(float deltaTime);
}
