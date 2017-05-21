using System.Collections.Generic;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;
using SharpCaster.Models.Enums;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Models.Metadata
{
    public interface IMetadata
    {
        List<ChromecastImage> images { get; set; }
        [JsonConverter(typeof(MetadataTypeEnumConverter))]
        MetadataTypeEnum metadataType { get; set; }
        string title { get; set; }
    }
}
