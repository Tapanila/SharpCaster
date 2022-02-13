using Sharpcaster.Messages.Chromecast;
using Sharpcaster.Models.Media;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Media status message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class MediaStatusMessage : StatusMessage<IEnumerable<MediaStatus>>
    {
    }
}
