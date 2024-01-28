﻿
using MmoNet.Core.Network.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace MmoNet.Core.Network.Serializers;
public class JsonSerializer(IPacketRegistry packetRegistry) : ISerializer {
    readonly IPacketRegistry packetRegistry = packetRegistry;

    public IPacket Deserialize(byte[] bytes) {
        var json = Encoding.UTF8.GetString(bytes);
        var jsonObject = JObject.Parse(json);
        var packetId = GetPacketId(jsonObject);
        var packetType = packetRegistry.GetPacketType(packetId);
        return (IPacket)jsonObject.ToObject(packetType)
               ?? throw new ArgumentException($"Packet deserialization failed: received json: {json}");
    }

    public byte[] Serialize(IPacket packet) {
        var json = JsonConvert.SerializeObject(packet, new JsonSerializerSettings() {
            NullValueHandling = NullValueHandling.Ignore,
            // convert all CamelCase to camelCase
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        return Encoding.UTF8.GetBytes(json);
    }

    int GetPacketId(JObject jsonObject) {
        var packetId = jsonObject["packetId"]?.Value<int>();
        if (packetId == null) {
            throw new ArgumentException($"Packet deserialization failed: received json: {jsonObject}");
        }
        return packetId.Value;
    }
}
