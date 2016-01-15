using Newtonsoft.Json;
using ProtoBuf;

namespace SharpCaster.Helpers
{
    [ProtoContract]
    public class CastMessage
    {
        [ProtoMember(1, IsRequired = true, Name = "protocol_version")]
        public int ProtocolVersion = 0;
        [ProtoMember(2, IsRequired = true, Name = "source_id")]
        public string SourceId;
        [ProtoMember(3, IsRequired = true, Name = "destination_id")]
        public string DestinationId;
        [ProtoMember(4, IsRequired = true, Name = "namespace")]
        public string Namespace;
        [ProtoMember(5, IsRequired = true, Name = "payload_type")]
        public int PayloadType = 0;
        [ProtoMember(6, IsRequired = false, Name = "payload_utf8")]
        public string PayloadUtf8;
        [ProtoMember(7, IsRequired = false, Name = "payload_binary")]
        public byte[] PayloadBinary;

        public string GetJsonType()
        {
            dynamic stuff = JsonConvert.DeserializeObject(PayloadUtf8);

            return stuff.type;
        }
    }
}