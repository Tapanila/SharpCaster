using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Linq;


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

            dynamic stuff = JsonConvert.DeserializeObject(PayloadUtf8);

            return stuff.type;
        }
    }
}
