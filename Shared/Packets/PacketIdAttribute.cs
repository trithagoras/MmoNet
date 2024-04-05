using System;

namespace MmoNet.Shared.Packets {
    /// <summary>
    /// Assign a unique packet id to a packet class. The IDs [1, 2, 3] are reserved for IdPacket, OkPacket and DenyPacket.
    /// </summary>
    /// <param name="id"></param>
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketIdAttribute : Attribute {
        public PacketIdAttribute(int id) {
            if (id < 4) {
                throw new ArgumentException("Packet IDs [1, 2, 3] are reserved for IdPacket, OkPacket and DenyPacket.");
            }
            Id = id;
        }

        public int Id { get; private set; }
    }
}
