using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Models.Media {
    [DataContract]
    public class QueueData {
        [DataMember(Name = "repeatMode")]
        public string RepeatMode { get; set; }

        [DataMember(Name = "shuffle")]
        public bool IsShuffle { get; set; }

        [DataMember(Name = "startIndex")]
        public long StartIndex { get; set; }

        [DataMember(Name = "startTime")]
        public long StartTime { get; set; }
    }
}
