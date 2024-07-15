using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Seek message
    /// </summary>
    [DataContract]
    public class SeekMessage : MediaSessionMessage
    {
        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        [DataMember(Name = "currentTime")]
        public double CurrentTime { get; set; }
    }
}
