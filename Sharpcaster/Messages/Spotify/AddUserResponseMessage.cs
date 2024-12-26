using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    [ReceptionMessage]
    public class AddUserResponseMessage : Message
    {
        public AddUserResponseMessage()
        {
            Type = "addUserResponse";
        }

        [JsonPropertyName("payload")]
        public AddUserResponseMessagePayload Payload { get; set; }
    }
    public class AddUserResponseMessagePayload
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("statusString")]
        public string StatusString { get; set; }
        [JsonPropertyName("spotifyError")]
        public int SpotifyError { get; set; }
        [JsonPropertyName("user")]
        public AddUserResponseMessagePayloadUser User { get; set; }
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }
    }
    public class AddUserResponseMessagePayloadUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
