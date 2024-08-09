using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    public class GetInfoMessage: MessageWithId
    {
        public string remoteName { get; set; }
        public string deviceId { get; set; }
        public bool deviceAPI_isGroup { get; set; }
    }
}
