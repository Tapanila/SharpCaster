using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media metadata
    /// </summary>
    [DataContract]
    [KnownType(typeof(MediaMetadata))]
    [KnownType(typeof(MovieMetadata))]
    public class MediaMetadata
    {
        /// <summary>
        /// Gets or sets the metadata type
        /// </summary>
        [DataMember(Name = "metadataType")]
        public MetadataType MetadataType { get; set; } = MetadataType.Default;

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle
        /// </summary>
        [DataMember(Name = "subtitle")]
        public string SubTitle { get; set; }

        /// <summary>
        /// Gets or sets the images
        /// </summary>
        [DataMember(Name = "images")]
        public Image[] Images { get; set; }
    }
}
