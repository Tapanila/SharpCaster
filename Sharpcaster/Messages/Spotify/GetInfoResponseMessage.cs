using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.Spotify;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    [ReceptionMessage]
    public class GetInfoResponseMessage : StatusMessage<SpotifyStatus>
    {
        public GetInfoResponseMessage()
        {
            Type = "getInfoResponse";
        }

        [DataMember(Name = "payload")]
        public SpotifyStatus Payload { get; set; }
    }
}
