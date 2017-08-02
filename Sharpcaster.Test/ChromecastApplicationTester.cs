using Sharpcaster.Core.Interfaces;
using System.Linq;
using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastApplicationTester
    {
        [Fact]
        public async void ConnectToChromecastAndLaunchApplication()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);

            var chromecast = chromecasts.First();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);

            status = await client.LaunchApplicationAsync("B3419EF5");
            //We make sure that the application was launched succesfully
            Assert.Equal(status.Applications[0].AppId, "B3419EF5");
        }

        [Fact]
        public async void ConnectToChromecastAndLaunchApplicationTwice()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);

            var chromecast = chromecasts.First();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5", false);

            //Relaunched the application on 2nd command
            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Fact]
        public async void ConnectToChromecastAndLaunchApplicationOnceAndJoinIt()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);

            var chromecast = chromecasts.First();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5", true);

            //Joined the existing session of the application on 2nd command
            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }
    }
}
