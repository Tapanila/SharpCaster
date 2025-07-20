namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Stop media message
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.StopRequest">Google Cast StopRequest Documentation</see>
    public class StopMediaMessage : MediaSessionMessage
    {
        public StopMediaMessage()
        {
            Type = "STOP";
        }
    }
}
