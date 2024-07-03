using System.Runtime.Serialization;

namespace Sharpcaster.Models.Queue
{
    [DataContract]
    public class QueueItem
    {
        /// <summary>
        /// Gets or sets the item identifier
        /// </summary>
        [DataMember(Name = "itemId", EmitDefaultValue = false)]
        public long? ItemId { get; set; }
        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [DataMember(Name = "media")]
        public Media.Media Media { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether autoplay is enabled or not
        /// </summary>
        [DataMember(Name = "autoPlay")]
        public bool? IsAutoPlay { get; set; }

        [DataMember(Name = "orderId", EmitDefaultValue = false)]
        public long? OrderId { get; set; }

        [DataMember(Name = "startTime", EmitDefaultValue = false)]
        public long? StartTime { get; set; }

        [DataMember(Name = "preloadTime", EmitDefaultValue = false)]
        public long? PreloadTime { get; set; }

        [DataMember(Name = "activeTrackIds", EmitDefaultValue = false)]
        public long[] ActiveTrackIds { get; set; }
    }
}
