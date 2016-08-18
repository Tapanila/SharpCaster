using System.Threading.Tasks;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Controllers
{
    public class SharpCasterDemoController : BaseController
    {

        public SharpCasterDemoController(ChromeCastClient client)
            : base(client, "B3419EF5")
        {
        }

        public async Task Play()
        {
            await Client.MediaChannel.Play();
        }

        public async Task Pause()
        {
            await Client.MediaChannel.Pause();
        }

        public async Task VolumeUp(float amount = 0.05f)
        {
            await Client.MediaChannel.IncreaseVolume(amount);
        }

        public async Task VolumeDown(float amount = 0.05f)
        {
            await Client.MediaChannel.DecreaseVolume(amount);
        }

        public async Task LoadMedia(
            string mediaUrl,
            string contentType = "application/vnd.ms-sstr+xml",
            Metadata metadata = null,
            string streamType = "BUFFERED",
            double duration = 0D,
            object customData = null,
            Track[] tracks = null,
            int[] activeTrackIds = null,
            bool autoPlay = true,
            double currentTime = 0.0)
        {
            await
                Client.MediaChannel.LoadMedia(mediaUrl, contentType, metadata, streamType, duration, customData, tracks,
                    activeTrackIds, autoPlay, currentTime);
        }

        public async Task Seek(double seconds)
        {
            await Client.MediaChannel.Seek(seconds);
        }

        public async Task SetMute(bool muted)
        {
            await Client.MediaChannel.SetMute(muted);
        }

        public async Task SetVolume(float f)
        {
            await Client.MediaChannel.SetVolume(f);
        }
    }
}
