using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Movie metadata
    /// </summary>
    [DataContract]
    public class MovieMetadata : MediaMetadata
    {
        /// <summary>
        /// Initializes a new instance of MovieMetadata class
        /// </summary>
        public MovieMetadata()
        {
            MetadataType = MetadataType.Movie;
        }

        /// <summary>
        /// Gets or sets the studio
        /// </summary>
        [DataMember(Name = "studio")]
        public string Studio { get; set; }
    }
}
