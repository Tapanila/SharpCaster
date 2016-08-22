using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class StopMediaRequest : RequestWithId
    {
        public StopMediaRequest(long mediaSessionId)
            : base("STOP")
        {
            MediaSessionId = mediaSessionId;
        }

        [DataMember(Name = "mediaSessionId")]
        public long MediaSessionId { get; set; }
    }
}
