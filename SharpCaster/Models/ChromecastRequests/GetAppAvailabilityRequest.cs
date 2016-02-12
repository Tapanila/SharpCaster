using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class GetAppAvailabilityRequest : RequestWithId
    {
        public GetAppAvailabilityRequest(string[] appIds)
            : base("GET_APP_AVAILABILITY")
        {
            ApplicationId = appIds;
        }

        [DataMember(Name = "appId")]
        public string[] ApplicationId { get; set; }
    }
}