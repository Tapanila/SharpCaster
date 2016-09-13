using System.Collections.Generic;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Models.Metadata
{
    public interface IMetadata
    {
        List<ChromecastImage> images { get; set; }
        int metadataType { get; set; }
        string title { get; set; }
    }
}
