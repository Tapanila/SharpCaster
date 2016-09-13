using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class MediaStatusRequest : RequestWithId

    {
        public MediaStatusRequest() : base("GET_STATUS")
        {
        }
    }
}
