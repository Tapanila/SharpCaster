using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PauseRequest : MediaRequest
    {
        public PauseRequest(long mediaSessionId)
            : base("PAUSE", mediaSessionId)
        {
        }
    }
}