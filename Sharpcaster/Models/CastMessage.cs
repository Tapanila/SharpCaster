using Google.Protobuf;
using Sharpcaster;
using System;
using System.Linq;
using System.Text.Json;

namespace Extensions.Api.CastChannel
{
    public sealed partial class CastMessage
    {
        partial void OnConstruction()
        {
            DestinationId = DefaultIdentifiers.DESTINATION_ID;
            SourceId = DefaultIdentifiers.SENDER_ID;
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
                var header = BitConverter.GetBytes((uint)bytes.Length);
                var dataToSend = header.Reverse().ToList();
                dataToSend.AddRange(bytes.ToList());
                bytes = dataToSend.ToArray();
            }

            return bytes;
        }

        public string GetJsonType()
        {
            if (string.IsNullOrEmpty(PayloadUtf8))
            {
                return string.Empty;
            }

            using (var document = JsonDocument.Parse(PayloadUtf8))
            {
                if (document.RootElement.TryGetProperty("type", out JsonElement typeElement))
                {
                    return typeElement.GetString() ?? string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
