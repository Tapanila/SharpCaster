using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public abstract class RequestWithId : Request
    {
        protected RequestWithId(string requestType)
            : base(requestType)
        {
            RequestId = RequestIdProvider.GetNext();
        }

        [DataMember(Name = "requestId")]
        public int RequestId { get; set; }
    }
}