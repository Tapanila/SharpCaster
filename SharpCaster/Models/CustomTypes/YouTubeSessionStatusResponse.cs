using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Models.CustomTypes
{
    [DataContract]
    public class YouTubeSessionStatusResponse
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "data")]
        public YouTubeSessionStatusDataResponse Data { get; set; }

    }
    [DataContract]
    public class YouTubeSessionStatusDataResponse
    {
        [DataMember(Name = "screenId")]
        public string ScreenId { get; set; }
    }
}
