using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class NextRequest : MediaRequest
    {
        public NextRequest(long mediaSessionId, int? requestId = null)
            : base("NEXT", mediaSessionId, requestId)
        {
        }
    }
}