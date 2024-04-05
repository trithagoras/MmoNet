using MmoNet.Shared.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace MmoNet.Shared.Serializers {
    public class JsonSerializer : ISerializer {
        readonly IPacketRegistry packetRegistry;

        public JsonSerializer(IPacketRegistry packetRegistry) {
            this.packetRegistry = packetRegistry;
        }

        public IPacket Deserialize(byte[] bytes) {
            var json = Encoding.UTF8.GetString(bytes);
            var jsonObject = JObject.Parse(json);
            var packetId = GetPacketId(jsonObject);
            var packetType = packetRegistry.GetPacketType(packetId);
            return (IPacket?)jsonObject.ToObject(packetType)
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
}
