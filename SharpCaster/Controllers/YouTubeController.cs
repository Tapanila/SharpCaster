using System;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Extensions;

namespace SharpCaster.Controllers
{
    public class YouTubeController : BaseMediaController
    {
        public event EventHandler<string> ScreenIdChanged;
        public YouTubeController(ChromeCastClient client) : base(client, "233637DE")
        {
            client.Channels.GetYouTubeChannel().ScreenIdChanged += OnScreenIdChanged;
        }

        private void OnScreenIdChanged(object sender, string s)
        {
            ScreenIdChanged?.Invoke(this, s);
        }
    }

    public static class YouTubeControllerExtensions
    {
        public static async Task<YouTubeController> LaunchYouTube(this ChromeCastClient client)
        {
            client.MakeSureChannelExist(new YouTubeChannel(client));
            var controller = new YouTubeController(client);
            await controller.LaunchApplication();
            return controller;
        }
    }
}
