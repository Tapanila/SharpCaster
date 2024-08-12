using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    public class GetInfoMessage: Message
    {
        [DataMember(Name = "payload")]
        public GetInfoMessagePayload Payload { get; set; }
        public GetInfoMessage()
        {
            Type = "getInfo";
        }
    }
}
