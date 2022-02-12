using System.Runtime.Serialization;

namespace Sharpcaster.Core.Messages.Heartbeat
{
    /// <summary>
    /// Pong message
    /// </summary>
    [DataContract]
    class PongMessage : Message
    {
    }
}
