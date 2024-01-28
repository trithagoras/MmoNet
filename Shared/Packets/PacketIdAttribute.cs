namespace MmoNet.Shared.Packets;

/// <summary>
/// Assign a unique packet id to a packet class. The IDs [1, 2, 3] are reserved for IdPacket, OkPacket and DenyPacket.
/// </summary>
/// <param name="id"></param>
[AttributeUsage(AttributeTargets.Class)]
public class PacketIdAttribute(int id) : Attribute {
    public int Id { get; private set; } = id;
}
