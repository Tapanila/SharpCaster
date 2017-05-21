using System.Threading.Tasks;
using SharpCaster.Interfaces;

namespace SharpCaster.Controllers
{
    public abstract class BaseMediaController : BaseController, IMediaController
    {
        protected BaseMediaController(ChromeCastClient client, string applicationId)
            : base(client, applicationId)
        {
            
        }

        public virtual async Task Play()
        {
            await Client.MediaChannel.Play();
        }

        public virtual async Task Pause()
        {
            await Client.MediaChannel.Pause();
        }

        public virtual async Task Seek(double seconds)
        {
            await Client.MediaChannel.Seek(seconds);
        }

        public virtual async Task Stop()
        {
            await Client.MediaChannel.Stop();
        }

        public virtual async Task Next()
        {
            await Client.MediaChannel.Next();
        }

        public virtual async Task Previous()
        {
            await Client.MediaChannel.Previous();
        }

        public async Task VolumeUp(double amount = 0.05)
        {
            await Client.ReceiverChannel.IncreaseVolume(amount);
        }

        public async Task VolumeDown(double amount = 0.05)
        {
            await Client.ReceiverChannel.DecreaseVolume(amount);
        }

        public async Task SetMute(bool muted)
        {
            await Client.ReceiverChannel.SetMute(muted);
        }

        public async Task SetVolume(double volume)
        {
            await Client.ReceiverChannel.SetVolume(volume);
        }
    }
}
