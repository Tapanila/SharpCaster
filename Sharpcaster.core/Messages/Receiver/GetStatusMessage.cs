using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Receiver
{
    /// <summary>
    /// Get status message
    /// </summary>
    [DataContract]
    class GetStatusMessage : MessageWithId
    {
    }
}
