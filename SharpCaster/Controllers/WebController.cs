using System.Collections.Generic;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Extensions;

namespace SharpCaster.Controllers
{
    public class WebController : BaseController
    {
        public static string WebAppId = "5CB45E5A";
        public WebController(ChromeCastClient client) : base(client, WebAppId)
        {
            
        }

        public async Task LoadUrl(string url, string type = "iframe")
        {
            await Client.Channels.GetWebChannel().LoadUrl(url, type);
        }
        
    }
    
    public static class WebControllerExtensions
    {
        public static async Task<WebController> LaunchWeb(this ChromeCastClient client)
        {
            client.MakeSureChannelExist(new WebChannel(client));
            var controller = new WebController(client);
            await controller.LaunchApplication();
            return controller;
        }  
    }    
}
