using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class WebRequest : RequestWithId 
    {
        public WebRequest(string appId, string strUrl, string type) : base(type)
        {
            ApplicationId = appId;
            Url = strUrl;
        }

        [DataMember(Name = "appId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
