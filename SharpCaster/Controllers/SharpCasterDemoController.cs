using System;
using System.Threading.Tasks;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Models.Metadata;

namespace SharpCaster.Controllers
{
    public class SharpCasterDemoController : BaseMediaController
    {
        public static string SharpCasterApplicationId = "B3419EF5";
        public SharpCasterDemoController(ChromeCastClient client)
            : base(client, SharpCasterApplicationId)
        {
        }

        public async Task LoadMedia(
            string mediaUrl,
            string contentType = "application/vnd.ms-sstr+xml",
            IMetadata metadata = null,
            string streamType = "BUFFERED",
            double duration = 0D,
            object customData = null,
            Track[] tracks = null,
            int[] activeTrackIds = null,
            bool autoPlay = true,
            double currentTime = 0.0)
        {
            StreamType parsedStreamType;

            Enum.TryParse(streamType, out parsedStreamType);
            await LoadMedia(mediaUrl, contentType, metadata, parsedStreamType, duration, customData, tracks, activeTrackIds,
                autoPlay, currentTime);
        }
        public async Task LoadMedia(
            string mediaUrl,
            string contentType = "application/vnd.ms-sstr+xml",
            IMetadata metadata = null,
            StreamType streamType = StreamType.BUFFERED,
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

    public static class SharpCasterDemoControllerExtension
    {
        public static async Task<SharpCasterDemoController> LaunchSharpCaster(this ChromeCastClient client)
        {
            var controller = new SharpCasterDemoController(client);
            await controller.LaunchApplication();
            return controller;
        }
    }
}
