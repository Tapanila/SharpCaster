using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PlayRequest : MediaRequest
    {
        public PlayRequest(long mediaSessionId, int? requestId = null)
            : base("PLAY", mediaSessionId, requestId)
        {
        }
    }
}