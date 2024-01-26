namespace MmoNet.Core.States;
[Flags]
public enum State {
    None = 0,
    Entry = 1,
    Play = 2,
    Any = Entry | Play
}
