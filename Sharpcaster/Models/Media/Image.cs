using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Image
    /// </summary>
    [DataContract]
    public class Image
    {
        /// <summary>
        /// Gets or sets the image url
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the image height
        /// </summary>
        [DataMember(Name = "height")]
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the image width
        /// </summary>
        [DataMember(Name = "width")]
        public int? Width { get; set; }
    }
}
