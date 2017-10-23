using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Heartbeat
{
    /// <summary>
    /// Ping message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    class PingMessage : Message
    {
    }
}
