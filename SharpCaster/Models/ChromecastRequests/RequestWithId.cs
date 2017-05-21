using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public abstract class RequestWithId : Request
    {
        protected RequestWithId(string requestType, int? requestId = null)
            : base(requestType)
        {
            RequestId = requestId ?? RequestIdProvider.GetNext();
        }

        [DataMember(Name = "requestId")]
        public int RequestId { get; set; }
    }
}