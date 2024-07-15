using System;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Load cancelled message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class LoadCancelledMessage : MessageWithId
    {
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            throw new Exception("Load cancelled");
        }
    }
}
