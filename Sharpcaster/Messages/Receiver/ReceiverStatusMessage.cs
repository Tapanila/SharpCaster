using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.ChromecastStatus;

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
