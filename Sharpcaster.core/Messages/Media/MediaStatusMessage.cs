using Sharpcaster.Core.Messages.Chromecast;
using Sharpcaster.Core.Models.Media;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sharpcaster.Core.Messages.Media
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
