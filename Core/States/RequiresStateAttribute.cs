namespace MmoNet.Core.States;

[AttributeUsage(AttributeTargets.Method)]
public class RequiresStateAttribute<T>() : Attribute where T : ISessionState {
    public T State { get; private set; }
}
