using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class GetStatusRequest : RequestWithId
    {
        public GetStatusRequest(int? requestId = null)
            : base("GET_STATUS", requestId)
        {
        }
    }
}