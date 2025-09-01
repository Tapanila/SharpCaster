using Sharpcaster.Interfaces;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Chromecast
{
    /// <summary>
    /// Status message base class
    /// </summary>
    /// <typeparam name="TStatus">status type</typeparam>
    [ReceptionMessage]
    public abstract class StatusMessage<TStatus> : MessageWithId
    {
        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [JsonPropertyName("status")]
        public TStatus Status { get; set; }
    }
}
