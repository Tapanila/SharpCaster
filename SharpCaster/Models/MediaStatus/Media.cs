using Newtonsoft.Json;
using SharpCaster.JsonConverters;
using SharpCaster.Models.Metadata;

namespace SharpCaster.Models.MediaStatus
{
    public class Media
    {
        public string contentId { get; set; }
        public string contentType { get; set; }
        public string streamType { get; set; }
        public double duration { get; set; }
        [JsonConverter(typeof(MetadataTypeConverter))]
        public IMetadata metadata { get; set; }
        public object customData { get; set; }
    }
}