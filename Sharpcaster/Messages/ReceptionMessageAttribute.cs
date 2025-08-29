using System;

namespace Sharpcaster.Messages
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class ReceptionMessageAttribute : Attribute
    {
    }
}
