using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.MultiZone;

namespace Sharpcaster.Messages.Multizone
{
    /// <summary>
    /// Media status message
    /// </summary>
    [ReceptionMessage]
    public class MultizoneStatusMessage : StatusMessage<MultiZoneStatus>
    {
    }
}
