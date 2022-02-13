using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Get status message
    /// </summary>
    [DataContract]
    class GetStatusMessage : MessageWithId
    {
    }
}
