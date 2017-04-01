using System.Collections.Generic;
using System.Threading.Tasks;
using SharpCaster.Models;
using System.Linq;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Channels
{
    public class WebChannel : ChromecastChannel
    {
        public static string Urn = "urn:x-cast:com.url.cast";

        public WebChannel(ChromeCastClient client) : base(client, Urn)
        {
        }

        public async Task LoadUrl(string url, string type)
        {
            var req = new WebRequest(Client.CurrentApplicationSessionId, url, type);
            var reqJson = req.ToJson();
            await Write(MessageFactory.Load(Client.CurrentApplicationTransportId, reqJson));
        }     
    }
    public static class WebChannelExtension
    {
        public static WebChannel GetWebChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (WebChannel)channels.First(x => x.Namespace == WebChannel.Urn);
        }
    }
}
