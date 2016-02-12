using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class StopRequest : RequestWithId
    {
        public StopRequest(string sessionId)
            : base("STOP")
        {
            SessionId = sessionId;
        }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}