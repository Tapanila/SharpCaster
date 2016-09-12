using System.Collections.Generic;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Models.Metadata
{
    //Fields: https://developers.google.com/cast/docs/reference/chrome/chrome.cast.media.GenericMediaMetadata
    public class GenericMediaMetadata : IMetadata
    {
        public List<ChromecastImage> images { get; set; }
        public int metadataType { get; set; }
        public string releaseDate { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
    }
}