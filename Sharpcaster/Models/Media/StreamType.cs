namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Stream type
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.StreamType">Google Cast StreamType Documentation</see>
    public enum StreamType
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Live
        /// </summary>
        Live,

        /// <summary>
        /// Buffered
        /// </summary>
        Buffered
    }
}
