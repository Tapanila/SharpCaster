using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Media session message
    /// </summary>
    [DataContract]
    public abstract class MediaSessionMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the media session identifier
        /// </summary>
        [DataMember(Name = "mediaSessionId")]
        public long MediaSessionId { get; set; }
    }
}
