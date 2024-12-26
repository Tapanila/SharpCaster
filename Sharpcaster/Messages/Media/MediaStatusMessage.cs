using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.Media;
using System.Collections.Generic;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Media status message
    /// </summary>
    [ReceptionMessage]
    public class MediaStatusMessage : StatusMessage<IEnumerable<MediaStatus>>
    {
    }
}
