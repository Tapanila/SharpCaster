using System.Collections.Generic;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;
using SharpCaster.Models.Enums;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Models.Metadata
{
    //Fields: https://developers.google.com/cast/docs/reference/chrome/chrome.cast.media.TvShowMediaMetadata
    public class TvShowMediaMetadata : IMetadata
    {
        public TvShowMediaMetadata()
        {
            metadataType = MetadataType.TV_SHOW;
        }
        public int episode { get; set; }
        public List<ChromecastImage> images { get; set; }
        [JsonConverter(typeof(MetadataTypeEnumConverter))]
        public MetadataType metadataType { get; set; }
        public string originalAirdate { get; set; }
        public int season { get; set; }
        public string seriesTitle { get; set; }
        public string title { get; set; }
    }
}
