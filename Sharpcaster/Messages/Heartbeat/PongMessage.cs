using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Heartbeat
{
    /// <summary>
    /// Pong message
    /// </summary>
    [DataContract]
    public class PongMessage : Message
    {
    }
}
