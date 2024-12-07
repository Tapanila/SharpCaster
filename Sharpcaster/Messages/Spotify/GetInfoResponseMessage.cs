using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.Spotify;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    [ReceptionMessage]
    public class GetInfoResponseMessage : StatusMessage<SpotifyStatus>
    {
        public GetInfoResponseMessage()
        {
            Type = "getInfoResponse";
        }

        [JsonPropertyName("payload")]
        public SpotifyStatus Payload { get; set; }
    }
}
