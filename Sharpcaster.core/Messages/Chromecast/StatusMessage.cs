using Sharpcaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Chromecast
{
    /// <summary>
    /// Status message base class
    /// </summary>
    /// <typeparam name="TStatus">status type</typeparam>
    [DataContract]
    public abstract class StatusMessage<TStatus> : MessageWithId, IStatusMessage<TStatus>
    {
        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [DataMember(Name = "status")]
        public TStatus Status { get; set; }
    }
}
