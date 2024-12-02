using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    public class GetInfoMessage : MessageWithId
    {
        [DataMember(Name = "payload")]
        public GetInfoMessagePayload Payload { get; set; }
        public GetInfoMessage()
        {
            Type = "getInfo";
        }
    }

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
