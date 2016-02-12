using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PongRequest : Request
    {
        public PongRequest()
            : base("PONG")
        {
        }
    }
}