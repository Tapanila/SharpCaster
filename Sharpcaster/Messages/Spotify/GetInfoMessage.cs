using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    public class GetInfoMessage : MessageWithId
    {
        [JsonPropertyName("payload")]
        public GetInfoMessagePayload Payload { get; set; }
        public GetInfoMessage()
        {
            Type = "getInfo";
        }
    }

    public class GetInfoMessagePayload
    {
        [JsonPropertyName("deviceAPI_isGroup")]
        public bool DeviceAPI_isGroup { get; set; }

        [JsonPropertyName("deviceID")]
        public string DeviceId { get; set; }
        [JsonPropertyName("remoteName")]
        public string RemoteName { get; set; }
    }
}
