using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.MultiZone;
using Sharpcaster.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    [ReceptionMessage]
    public class GetInfoResponseMessage : StatusMessage<SpotifyStatus>
    {
        public GetInfoResponseMessage()
        {
            Type = "getInfoResponse";
            var i = 0;
        }

        [DataMember(Name = "payload")]
        public SpotifyStatus Payload { get; set; }
    }
}
