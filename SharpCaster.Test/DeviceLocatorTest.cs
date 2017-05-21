using SharpCaster.Services;
using Xunit;

namespace SharpCaster.Test
{
    public class DeviceLocatorTest
    {
        private readonly ChromecastService _chromecastService;
        public DeviceLocatorTest()
        {
            _chromecastService = ChromecastService.Current;
        }

        [Fact]
        public async void SearchChromecast()
        {
            var devices = await _chromecastService.StartLocatingDevices();
           Assert.True(devices.Count > 0);
        }
    }
}
