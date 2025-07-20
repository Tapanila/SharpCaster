namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Player state types from chrome.cast.media.PlayerState
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.PlayerState">Google Cast PlayerState Documentation</see>
    public enum PlayerStateType
    {
        Buffering,
        Idle,
        Paused,
        Loading,
        Playing
    }
}
