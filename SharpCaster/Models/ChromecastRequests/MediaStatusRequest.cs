using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class MediaStatusRequest : RequestWithId

    {
        public MediaStatusRequest(int? requestId = null) : base("GET_STATUS", requestId)
        {
        }
    }
}
