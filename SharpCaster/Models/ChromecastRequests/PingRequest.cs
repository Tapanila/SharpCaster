using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PingRequest : Request
    {
        public PingRequest()
            : base("PING")
        {
        }
    }
}