using Sharpcaster.Core.Messages.Chromecast;
using Sharpcaster.Core.Models.ChromecastStatus;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Receiver
{
    /// <summary>
    /// Receiver status message
    /// </summary>
    [DataContract]
    public class ReceiverStatusMessage : StatusMessage<ChromecastStatus>
    {
    }
}
