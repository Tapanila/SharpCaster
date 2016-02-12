using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PauseRequest : RequestWithId
    {
        public PauseRequest()
            : base("PAUSE")
        {
        }
    }
}