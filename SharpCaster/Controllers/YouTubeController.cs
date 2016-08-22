using System;
using System.Threading.Tasks;
using SharpCaster.Extensions;

namespace SharpCaster.Controllers
{
    public class YouTubeController : BaseController
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
