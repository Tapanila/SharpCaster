﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class ChromecastStatus
    {
        [JsonProperty("applications")]
        public List<ChromecastApplication> Applications { get; set; }
        [JsonProperty("isActiveInput")]
        public bool IsActiveInput { get; set; }
        [JsonProperty("isStandBy")]
        public bool IsStandBy { get; set; }
        [JsonProperty("volume")]
        public Volume Volume { get; set; }
        [JsonIgnore]
        public ChromecastApplication Application { get => Applications.FirstOrDefault(); }
    }
}
