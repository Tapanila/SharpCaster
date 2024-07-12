using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Connection
{
    /// <summary>
    /// Close message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class CloseMessage : Message
    {
    }
}
