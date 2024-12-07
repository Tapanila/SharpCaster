using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Heartbeat
{
    /// <summary>
    /// Ping message
    /// </summary>
    [ReceptionMessage]
    public class PingMessage : Message
    {
    }
}
