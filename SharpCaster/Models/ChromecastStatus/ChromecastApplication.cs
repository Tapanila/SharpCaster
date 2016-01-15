using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastStatus
{
    public class ChromecastApplication
    {
        public string appId { get; set; }
        public string displayName { get; set; }
        public List<Namespace> namespaces { get; set; }
        public string sessionId { get; set; }
        public string statusText { get; set; }
        public string transportId { get; set; }
    }
}