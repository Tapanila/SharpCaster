using System;
using System.Threading.Tasks;
using SharpCaster.Extensions;

namespace SharpCaster.Controllers
{
    public class PlexController : BaseController
    {
        public PlexController(ChromeCastClient client) : base(client, "9AC194DC")
        {
        }

        public async Task Play()
        {
            await Client.Channels.GetPlexChannel().Play();
        }

        public async Task Pause()
        {
            await Client.Channels.GetPlexChannel().Pause();
        }

        public async Task Seek(double seconds)
        {
            await Client.Channels.GetPlexChannel().Seek(seconds);
        }
        
    }
}
