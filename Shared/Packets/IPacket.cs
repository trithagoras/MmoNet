using System;

namespace MmoNet.Shared.Packets { 
    public interface IPacket {
        public int PacketId { get; }
        public Guid SessionId { get; set; }
    }
}
