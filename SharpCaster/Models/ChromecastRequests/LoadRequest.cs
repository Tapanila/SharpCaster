using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class LoadRequest : RequestWithId
    {
        public LoadRequest(string sessionId, MediaData media, bool autoPlay, double currentTime,
            object customData = null, int[] activeTrackIds = null)
            : base("LOAD")
        {
            SessionId = sessionId;
            Media = media;
            AutoPlay = autoPlay;
            CurrentTime = currentTime;
            Customdata = customData;
            ActiveTrackIds = activeTrackIds;
        }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; private set; }

        [DataMember(Name = "media")]
        public MediaData Media { get; private set; }

        [DataMember(Name = "autoplay")]
        public bool AutoPlay { get; private set; }

        [DataMember(Name = "currentTime")]
        public double CurrentTime { get; private set; }

        [DataMember(Name = "customData")]
        public object Customdata { get; }

        [DataMember(Name = "activeTrackIds")]
        public int[] ActiveTrackIds { get; set; }
    }
}