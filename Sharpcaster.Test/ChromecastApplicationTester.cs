using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastApplicationTester
    {
        [Fact]
        public async Task ConnectToChromecastAndLaunchApplication()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);

            status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal("B3419EF5", status.Applications[0].AppId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationTwice()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5", true);

            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }


        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationTwiceWithoutJoining()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5", false);

            // ??????
            // My JBL Device (almost every time - but not always ) makes a new ID here!!!! (The other device - ChromecastAudio DOES NOT!?)
            //Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationAThenLaunchApplicationB()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("A9BCCB7C"); //Youtube

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(chromecast);

            status = await client.LaunchApplicationAsync("B3419EF5"); //My sample Application

            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationOnceAndJoinIt()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5");

            
            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
            
        }
    }
}
