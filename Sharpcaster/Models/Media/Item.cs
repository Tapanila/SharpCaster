using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Item
    /// </summary>
    [DataContract]
    public class Item
    {
        /// <summary>
        /// Gets or sets the item identifier
        /// </summary>
        [DataMember(Name = "itemId")]
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [DataMember(Name = "media")]
        public Media Media { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether autoplay is enabled or not
        /// </summary>
        [DataMember(Name = "autoplay")]
        public bool Autoplay { get; set; }
    }
}
