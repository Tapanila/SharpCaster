using System;
using System.Threading.Tasks;

namespace SharpCaster.MediaControllers
{
    [Flags]
    public enum SupportedCommand
    {
        Play,
        Pause,
        Seek,
        Stop,
        Previous,
        Next,
        Shuffle,
        Repeat,
        GetMediaStatus,
        LoadSmoothStreaming,
        SkipTo, //This is also a Plex command but I assume it works the same as the PlayIndex command in DS Audio

        // Should these rare commands be in the common interface?
        PlexShowDetails,
        PlexRefreshPlayQueue,
        PlexSetQuality,
        PlexSetStream,

        DSAudioReplayCurrent,
        DSAudioUpdatePlaylist
    }

    public enum RepeatMode
    {
        /// <summary>
        /// Do not repeat
        /// </summary>
        None,

        /// <summary>
        /// Repeat the current track
        /// </summary>
        One,

        /// <summary>
        /// Repeat after the last song in the playlist finished
        /// </summary>
        All
    }

    public interface IMediaController
    {
        /// <summary>
        /// Get a list of supported commands
        /// </summary>
        /// <returns><see cref="FlagsAttribute">Bitflags</see> to check against the <see cref="SupportedCommand" /> enum</returns>
        /// <remarks>Commands that are not supported are still available in the code but they will throw an exception</remarks>
        SupportedCommand SupportedCommands { get; }

        bool SupportsCommand(SupportedCommand commandToCheck);

        /// <summary>
        /// If apps use a special namespace other than <see cref="DefaultMediaMessageFactory.DefaultMediaUrn"/> then specify it here
        /// </summary>
        string SpecificNamespace { get; }
        
        /// <summary>
        /// Default app to start for this controller if it is not running yet
        /// </summary>
        string DefaultAppId { get; }

        Task Play();

        Task Pause();

        Task Stop();

        Task Seek(double seconds);

        Task Previous();

        Task Next();

        Task Shuffle(bool enabled);

        Task Repeat(RepeatMode mode);

        Task GetMediaStatus();

        //TODO I think that the customData can be removed here if we have custom media controllers
        //TODO I kept it for now, for compatibility
        Task LoadSmoothStreaming(string streamUrl, object customData = null);
    }
}
