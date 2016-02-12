using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public abstract class Request
    {
        protected Request(string requestType)
        {
            RequestType = requestType;

        }

        [DataMember(Name = "type")]
        public string RequestType { get; set; }


        public string ToJson()
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}