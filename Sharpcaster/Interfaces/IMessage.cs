namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for a message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the message type
        /// </summary>
        string Type { get; }
    }
}
