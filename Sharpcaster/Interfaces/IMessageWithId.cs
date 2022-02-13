namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for messages with request identifier
    /// </summary>
    public interface IMessageWithId : IMessage
    {
        /// <summary>
        /// Gets the request identifier
        /// </summary>
        int RequestId { get; }
    }
}
