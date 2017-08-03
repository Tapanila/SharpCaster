using System;
using System.Runtime.Serialization;

namespace Sharpcaster.Core.Messages.Media
{
    /// <summary>
    /// Load cancelled message
    /// </summary>
    [DataContract]
    class LoadCancelledMessage : MessageWithId
    {
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            throw new Exception("Load cancelled");
        }
    }
}
