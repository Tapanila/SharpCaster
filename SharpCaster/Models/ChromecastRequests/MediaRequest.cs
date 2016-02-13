using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    public class MediaRequest : RequestWithId
    {
        public MediaRequest(string requestType, long mediaSessionId) : base(requestType)
        {
            MediaSessionId = mediaSessionId;
        }

        [DataMember(Name = "mediaSessionId")]
        public long MediaSessionId { get; set; }
    }
}
