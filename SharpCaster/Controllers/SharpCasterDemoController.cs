using System.Threading.Tasks;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Controllers
{
    public class SharpCasterDemoController : BaseMediaController
    {

        public SharpCasterDemoController(ChromeCastClient client)
            : base(client, "B3419EF5")
        {
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
    }
}
