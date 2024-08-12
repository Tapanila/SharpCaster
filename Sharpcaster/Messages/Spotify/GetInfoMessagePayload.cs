using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    public class GetInfoMessagePayload
    {
        [DataMember(Name = "deviceAPI_isGroup")]
        public bool DeviceAPI_isGroup { get; set; }

        [DataMember(Name = "deviceID")]
        public string DeviceId { get; set; }
        [DataMember(Name = "remoteName")]
        public string RemoteName { get; set; }
    }
}
