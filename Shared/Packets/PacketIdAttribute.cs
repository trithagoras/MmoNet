namespace MmoNet.Shared.Packets;

[AttributeUsage(AttributeTargets.Class)]
public class PacketIdAttribute(int id) : Attribute {
    public int Id { get; private set; } = id;
}
