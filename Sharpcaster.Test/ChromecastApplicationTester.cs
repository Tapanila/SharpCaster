using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastApplicationTester
    {
        [Fact]
        public async void ConnectToChromecastAndLaunchApplication()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);

            status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal("B3419EF5", status.Applications[0].AppId);
        }

        [Fact]
        public async void ConnectToChromecastAndLaunchApplicationTwice()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5", false);

            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Fact]
        public async void ConnectToChromecastAndLaunchApplicationOnceAndJoinIt()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5", true);

            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }
    }
}
