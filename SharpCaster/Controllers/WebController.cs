using System;
using System.Threading.Tasks;
using SharpCaster.Extensions;
using SharpCaster.Channels;
using System.Collections.Generic;
using System.Linq;

namespace SharpCaster.Controllers
{
    public class WebController : BaseController, IController 
    {
        public event EventHandler<string> ScreenIdChanged;
        public WebController(ChromeCastClient client) : base(client, "5CB45E5A")
        {
            client.Channels.GetWebChannel().ScreenIdChanged += OnScreenIdChanged;
        }

        private void OnScreenIdChanged(object sender, string s)
        {
            ScreenIdChanged?.Invoke(this, s);
        }
    }
    
    public static class WebControllerExtensions
    {
        public static async Task<WebController> LaunchWeb(this ChromeCastClient client)
        {
            client.Channels.Add(new WebChannel(client));
            var controller = new WebController(client);
            await controller.LaunchApplication();
            return controller;
        }
        
        public static WebChannel GetWebChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (WebChannel)channels.First(x => x.Namespace == "urn:x-cast:com.url.cast");
        }        
    }    
}
