using System;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Load failed message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class LoadFailedMessage : MessageWithId
    {
    }
}
