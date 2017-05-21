using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;
using SharpCaster.Models.Enums;

namespace SharpCaster.Models.MediaStatus
{
    //
    // https://developers.google.com/cast/docs/reference/messages#LoadFailed
    // https://developers.google.com/cast/docs/reference/messages#LoadCancelled
    // https://developers.google.com/cast/docs/reference/messages#InvalidRequest
    //
    public class ChromecastMediaError
    {
        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(MediaErrorTypeEnumConverter))]
        public MediaErrorTypeEnum MediaErrorType { get; set; }
        [JsonProperty("reason")]
        [JsonConverter(typeof(MediaErrorReasonEnumConverter))]
        public MediaErrorReasonEnum? Reason { get; set; }
        [JsonProperty("customData")]
        public object CustomData { get; set; }
    }


}
