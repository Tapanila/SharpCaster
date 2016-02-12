using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class LoadRequest : RequestWithId
    {
        public LoadRequest(string sessionId, MediaRequest media, bool autoPlay, double currentTime,
            object customData = null)
            : base("LOAD")
        {
            SessionId = sessionId;
            Media = media;
            AutoPlay = autoPlay;
            CurrentTime = currentTime;
            Customdata = customData;
        }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; private set; }

        [DataMember(Name = "media")]
        public MediaRequest Media { get; private set; }

        [DataMember(Name = "autoplay")]
        public bool AutoPlay { get; private set; }

        [DataMember(Name = "currentTime")]
        public double CurrentTime { get; private set; }

        [DataMember(Name = "customData")]
        public object Customdata { get; }
    }
}