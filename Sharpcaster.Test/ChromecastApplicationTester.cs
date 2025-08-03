using Sharpcaster.Models;
using Sharpcaster.Messages.Web;
using Sharpcaster.Extensions;
using Sharpcaster.Test.helper;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastApplicationTester(ITestOutputHelper output, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task ConnectToChromecastAndLaunchApplication()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal("B3419EF5", status.Application.AppId);

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationTwice()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Application.TransportId;
            await client.DisconnectAsync();

            _ = await client.ConnectChromecast(fixture.Receivers[0]);
            status = await client.LaunchApplicationAsync("B3419EF5", true);

            Assert.Equal(firstLaunchTransportId, status.Application.TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationTwiceWithoutJoining2()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Application.TransportId;
            await client.DisconnectAsync();

            _ = await client.ConnectChromecast(fixture.Receivers[0]);
            status = await client.LaunchApplicationAsync("B3419EF5", false);

            // My ChromecastAudio device keeps the same transport session here!
            Assert.Equal(firstLaunchTransportId, status.Application.TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationAThenLaunchApplicationB()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("233637DE");           //Something else

            var firstLaunchTransportId = status.Application.TransportId;
            await client.DisconnectAsync();

            _ = await client.ConnectChromecast(fixture.Receivers[0]);
            status = await client.LaunchApplicationAsync("B3419EF5");               //My sample Application

            Assert.NotEqual(firstLaunchTransportId, status.Application.TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchApplicationOnceAndJoinIt()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Application.TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal(firstLaunchTransportId, status.Application.TransportId);
        }

        [Fact]
        public async Task ConnectToChromecastAndLaunchWebPage()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(output, fixture.Receivers[0], "F7FD2183");

            var req = new WebMessage
            {
                Url = "https://mallow.fi/",
                Type = "load",
                SessionId = client.ChromecastStatus.Application.SessionId
            };

            var requestPayload = JsonSerializer.Serialize(req, SharpcasteSerializationContext.Default.WebMessage);

            await client.SendAsync(null, "urn:x-cast:com.boombatower.chromecast-dashboard", requestPayload, client.ChromecastStatus.Application.SessionId);
        }
    }
}
