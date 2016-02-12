using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class GetStatusRequest : RequestWithId
    {
        public GetStatusRequest()
            : base("GET_STATUS")
        {
        }
    }
}