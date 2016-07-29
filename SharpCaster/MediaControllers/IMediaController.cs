using System;
using System.Threading.Tasks;

namespace SharpCaster.MediaControllers
{
    [Flags]
    public enum SupportedCommand
    {
        Play,
        Pause,
        Stop,
        Seek,
        Previous,
        Next,
        Shuffle,
        GetMediaStatus,
        LoadSmoothStreaming
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

        Task Shuffle();

        Task GetMediaStatus();
    }
}
