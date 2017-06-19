using System.IO;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCaster.Extensions;

namespace Extensions.Api.CastChannel
{
    public sealed partial class CastMessage
    {
        partial void OnConstruction()
        {
            DestinationId = "receiver-0";
            SourceId = "sender-0";
        }
        public CastMessage(string destinationId, string sourceId)
        {
            OnConstruction();

            DestinationId = destinationId;
            SourceId = sourceId;
        }

        public byte[] ToProto(bool includeHeader = true)
        {
            var bytes = this.ToByteArray();

            if (includeHeader)
            {
                bytes = bytes.AddHeader();
            }

            return bytes;
        }

        public string GetJsonType()
        {
            if (string.IsNullOrEmpty(PayloadUtf8))
            {
                return string.Empty;
            }

            var jObject = JObject.Parse(PayloadUtf8);

            JToken jToken = null;
            if (jObject.TryGetValue("type", out jToken) == false)
            {
                return string.Empty;
            }

            return jToken.Value<string>();
        }
    }
}