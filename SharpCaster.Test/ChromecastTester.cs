using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCaster.Controllers;
using SharpCaster.Services;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SharpCaster.Test
{
    public class ChromecastTester
    {
        private ChromecastService _chromecastService;
        public ChromecastTester()
        {
            _chromecastService = ChromecastService.Current;
            var device = _chromecastService.StartLocatingDevices().Result;
            _chromecastService.ConnectToChromecast(device.First()).Wait(2000);
        }

        [Fact]
        public void TestingConnection()
        {
            Assert.NotNull(_chromecastService.ConnectedChromecast);
        }

        [Fact]
        public void TestingLaunchingSharpCasterDemo()
        {
            var sharpcasterController = _chromecastService.ChromeCastClient.LaunchSharpCaster().Wait(4000);
            Assert.NotNull(_chromecastService.ChromeCastClient.ChromecastStatus.Applications.First(x => x.AppId == SharpCasterDemoController.SharpCasterApplicationId));
        }
    }
}
