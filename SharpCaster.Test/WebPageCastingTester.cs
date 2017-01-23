using System.Linq;
using System.Threading.Tasks;
using SharpCaster.Controllers;
using SharpCaster.Services;
using Xunit;

namespace SharpCaster.Test
{
    public class WebPageCastingTester
    {
        private ChromecastService _chromecastService;
        public WebPageCastingTester()
        {
            _chromecastService = ChromecastService.Current;
            var device = _chromecastService.StartLocatingDevices().Result;
            _chromecastService.ConnectToChromecast(device.First()).Wait(2000);
        }

        [Fact]
        public async void TestingLaunchingSharpCasterDemo()
        {
            var controller =  await _chromecastService.ChromeCastClient.LaunchWeb();
            await Task.Delay(4000);
            Assert.NotNull(_chromecastService.ChromeCastClient.ChromecastStatus.Applications.First(x => x.AppId == WebController.WebAppId));
            await controller.LoadUrl("https://www.windytv.com/");
            await Task.Delay(4000);
            Assert.Equal(_chromecastService.ChromeCastClient.ChromecastStatus.Applications.First(x => x.AppId == WebController.WebAppId).StatusText,
                "Now Playing: https://www.windytv.com/");
        }
    }
}
