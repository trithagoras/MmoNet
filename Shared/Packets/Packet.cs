using System;
using System.Reflection;

namespace MmoNet.Shared.Packets {
    public abstract class Packet : IPacket {
        private int? cachedPacketId;

        public int PacketId {
            get {
                if (!cachedPacketId.HasValue) {
                    cachedPacketId = GetType().GetCustomAttribute<PacketIdAttribute>()?.Id;
                }
                return cachedPacketId!.Value;
            }
        }

        public Guid SessionId { get; set; }
    }
}