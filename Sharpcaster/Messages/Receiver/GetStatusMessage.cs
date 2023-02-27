using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Get status message
    /// </summary>
    [DataContract]
    public class GetStatusMessage : MessageWithId
    {
    }
}
