using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpcaster.Core.Models.ChromecastStatus
{
    public class Namespace
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
