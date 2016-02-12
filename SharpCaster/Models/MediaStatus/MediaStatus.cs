using System.Collections.Generic;

namespace SharpCaster.Models.MediaStatus
{
    public class MediaStatus
    {
        public string type { get; set; }
        public List<Status> status { get; set; }
        public int requestId { get; set; }
    }
}