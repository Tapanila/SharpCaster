using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.Media;
using System.Collections.Generic;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Media status message
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.MediaStatus">Google Cast MediaStatus Documentation</see>
    [ReceptionMessage]
    public class MediaStatusMessage : StatusMessage<IEnumerable<MediaStatus>>
    {
    }
}
