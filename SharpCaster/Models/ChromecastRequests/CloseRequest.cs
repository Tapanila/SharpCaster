using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class CloseRequest : Request
    {
        public CloseRequest()
            : base("CLOSE")
        {
        }
    }
}