
using MmoNet.Core.Network.Packets;

namespace Sample.Packets {
    public class PacketIdsExtension : PacketIds {
        public static int LoginPacket => 4;
        public static int LogoutPacket => 5;
    }
}
