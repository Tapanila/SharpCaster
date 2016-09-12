using System;
using System.Threading.Tasks;
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
}
