
namespace MmoNet.Core.States;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresStateAttribute(State requiredStates, State excludedStates = State.None) : Attribute {
    public State RequiredStates { get; private set; } = requiredStates;
    public State ExcludedStates { get; private set; } = excludedStates;
}
