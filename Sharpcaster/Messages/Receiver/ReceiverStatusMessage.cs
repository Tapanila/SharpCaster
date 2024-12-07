using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.ChromecastStatus;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Receiver status message
    /// </summary>
    [ReceptionMessage]
    public class ReceiverStatusMessage : StatusMessage<ChromecastStatus>
    {
    }
}
