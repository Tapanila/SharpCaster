using System.Runtime.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    [DataContract]
    public class PlaybackSessionModel
    {
        [DataMember(Name = "appAllowsGrouping")]
        public bool? AppAllowsGrouping { get; set; }
        [DataMember(Name = "isVideoContent")]
        public bool? IsVideoContent { get; set; }
        [DataMember(Name = "streamTransferSupported")]
        public bool? StreamTransferSupported { get; set; }
    }
}
