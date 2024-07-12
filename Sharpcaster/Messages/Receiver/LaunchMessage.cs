using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Launch message
    /// </summary>
    [DataContract]
    public class LaunchMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the application identifier
        /// </summary>
        [DataMember(Name = "appId")]
        public string ApplicationId { get; set; }
    }
}
