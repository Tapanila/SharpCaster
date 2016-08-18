using System.Threading.Tasks;
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
    }
}
