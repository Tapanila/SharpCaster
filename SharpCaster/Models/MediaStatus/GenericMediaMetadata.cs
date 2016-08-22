using System.Collections.Generic;

namespace SharpCaster.Models.MediaStatus
{
    public class GenericMediaMetadata
    {
        public int metadataType { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public List<ChromecastImage> images { get; set; }

        public string releaseDate { get; set; }
    }
}
