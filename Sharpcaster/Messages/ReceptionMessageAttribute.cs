using System;

namespace Sharpcaster.Messages
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    /// <summary>
    /// Attribute for received messages
    /// </summary>
    public class ReceptionMessageAttribute : Attribute
    {
    }
}
