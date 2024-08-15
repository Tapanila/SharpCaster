using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Spotify
{
    [DataContract]
    [ReceptionMessage]
    public class AddUserResponseMessage: Message
    {
        public AddUserResponseMessage()
        {
            Type= "addUserResponse";
        }

        [DataMember(Name = "payload")]
        public AddUserResponseMessagePayload Payload { get; set; }
    }
    [DataContract]
    public class AddUserResponseMessagePayload
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "statusString")]
        public string StatusString { get; set; }
        [DataMember(Name = "spotifyError")]
        public int SpotifyError { get; set; }
        [DataMember(Name = "user")]
        public AddUserResponseMessagePayloadUser User { get; set; }
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }
    }
    [DataContract]
    public class AddUserResponseMessagePayloadUser
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}
