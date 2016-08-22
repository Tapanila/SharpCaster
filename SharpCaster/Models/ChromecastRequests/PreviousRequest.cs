using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PreviousRequest : MediaRequest
    {
        public PreviousRequest(long mediaSessionId)
            : base("PREVIOUS", mediaSessionId)
        {
        }
    }
}