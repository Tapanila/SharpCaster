namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Repeat mode types from chrome.cast.media.RepeatMode
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.RepeatMode">Google Cast RepeatMode Documentation</see>
    public enum RepeatModeType
    {
        OFF,
        ALL,
        SINGLE,
        ALL_AND_SHUFFLE
    }
}
