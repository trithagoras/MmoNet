
namespace MmoNet.Core.Network.Packets; 
public class OkPacket : Packet {
    public override int PacketId => PacketIds.OkPacket;
    public object? Result { get; set; }
}
