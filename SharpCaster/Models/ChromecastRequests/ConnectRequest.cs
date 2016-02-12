using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class ConnectRequest : Request
    {
        public ConnectRequest()
            : base("CONNECT")
        {
        }
    }
}