using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Controllers;

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
            return (YouTubeChannel) channels.First(x => x.Namespace == "urn:x-cast:com.google.youtube.mdx");
        }
    }
}
