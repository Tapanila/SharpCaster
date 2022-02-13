using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Get or sets the media
    /// </summary>
    [DataContract]
    public class Media
    {
        /// <summary>
        /// Gets or sets the content identifier
        /// </summary>
        [DataMember(Name = "contentId")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the stream type
        /// </summary>
        [DataMember(Name = "streamType")]
        public StreamType StreamType { get; set; } = StreamType.Buffered;

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [DataMember(Name = "metadata")]
        public MediaMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the duration of the media
        /// </summary>
        [DataMember(Name = "duration")]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the custom data
        /// </summary>
        [DataMember(Name = "customData")]
        public IDictionary<string, string> CustomData { get; set; }
    }
}
