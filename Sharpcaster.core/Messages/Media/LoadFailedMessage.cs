using System;
using System.Runtime.Serialization;

namespace Sharpcaster.Core.Messages.Media
{
    /// <summary>
    /// Load failed message
    /// </summary>
    [DataContract]
    class LoadFailedMessage : MessageWithId
    {
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            throw new Exception("Load failed");
        }
    }
}
