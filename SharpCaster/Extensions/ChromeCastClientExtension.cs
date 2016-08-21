using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Controllers;
using SharpCaster.Models;

namespace SharpCaster.Extensions
{
    public static class ChromeCastClientExtension
    {
        public static async Task<SharpCasterDemoController> LaunchSharpCaster(this ChromeCastClient client)
        {
            var controller = new SharpCasterDemoController(client);
            await controller.LaunchApplication();
            return controller;
        }

        public static async Task<YouTubeController> LaunchYouTube(this ChromeCastClient client)
        {
            client.Channels.Add(new YouTubeChannel(client));
            var controller = new YouTubeController(client);
            await controller.LaunchApplication();
            return controller;
        }

        public static YouTubeChannel GetYouTubeChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (YouTubeChannel)channels.First(x => x.Namespace == MessageFactory.DialConstants.YouTubeUrn);
        }

        public static async Task<PlexController> LaunchPlex(this ChromeCastClient client)
        {
            client.Channels.Add(new PlexChannel(client));
            var controller = new PlexController(client);
            await controller.LaunchApplication();
            return controller;
        }

        public static PlexChannel GetPlexChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (PlexChannel)channels.First(x => x.Namespace == MessageFactory.DialConstants.PlexUrn);
        }
    }
}
