using System.Collections.Generic;

namespace SharpCaster.Models.MediaStatus
{
    public class MediaStatusResponse
    {
        public string type { get; set; }
        public List<MediaStatus> status { get; set; }
        public int requestId { get; set; }
    }
}