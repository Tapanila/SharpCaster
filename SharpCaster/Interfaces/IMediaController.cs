using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface  IMediaController : IController
    {
        /// <summary>
        /// Start playing
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <returns></returns>
        Task Play();

        /// <summary>
        /// Pause the media item
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <returns></returns>
        Task Pause();

        /// <summary>
        /// Go to a specific time in the media item
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <param name="seconds">Time in seconds</param>
        /// <returns></returns>
        Task Seek(double seconds);

        /// <summary>
        /// Stop playing
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <returns></returns>
        Task Stop();

        /// <summary>
        /// Play the next item in a playlist
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <returns></returns>
        Task Next();

        /// <summary>
        /// Play the previous item in a playlist
        /// </summary>
        /// <remarks>throws a NotSupportedException if the app does not support this command</remarks>
        /// <returns></returns>
        Task Previous();

        /// <summary>
        /// Increase the volume by an optional amount
        /// </summary>
        /// <param name="amount">Amount to increase the volume</param>
        /// <remarks>The standard volume increase is 0.05</remarks>
        /// <returns></returns>
        Task VolumeUp(double amount = 0.05);

        /// <summary>
        /// Decrease the volume by an optional amount
        /// </summary>
        /// <param name="amount">Amount to decrease the volume</param>
        /// <remarks>The standard volume decrease is 0.05</remarks>
        /// <returns></returns>
        Task VolumeDown(double amount = 0.05);

        /// <summary>
        /// Mute or unmute the audio
        /// </summary>
        /// <param name="muted">True to mute; False to unmute</param>
        /// <returns></returns>
        Task SetMute(bool muted);

        /// <summary>
        /// Directly set the volume to a specific value
        /// </summary>
        /// <remarks>The volume should be 0.0 &lt;= volume &lt;= 1.0</remarks>
        /// <param name="volume">Desired volume level</param>
        /// <returns></returns>
        Task SetVolume(double volume);
    }
}
