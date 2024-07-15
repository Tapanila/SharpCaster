using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Heartbeat
{
    /// <summary>
    /// Ping message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class PingMessage : Message
    {
    }
}
