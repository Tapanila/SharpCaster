using System.Runtime.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    [DataContract]
    public class MultiZoneStatus
    {

        [DataMember(Name = "devices")]
        public Device[] Devices { get; set; }
        [DataMember(Name = "isMultichannel")]
        public bool? IsMultichannel { get; set; }
        [DataMember(Name = "playbackSession")]
        public PlaybackSessionModel PlaybackSession { get; set; }
    }
}
