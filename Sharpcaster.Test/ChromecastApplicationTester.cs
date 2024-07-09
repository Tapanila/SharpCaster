using Sharpcaster.Models;
using Sharpcaster.Test.customChannel;
using Sharpcaster.Test.helper;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastApplicationTester : IClassFixture<ChromecastDevicesFixture> {

        private ITestOutputHelper output;
        public ChromecastApplicationTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture) {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplication(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal("B3419EF5", status.Applications[0].AppId);

            await client.DisconnectAsync();
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplicationTwice(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(receiver);
            status = await client.LaunchApplicationAsync("B3419EF5", true);

            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }


        [Theory(Skip = "This does not pass any more. Now my JBL reacts as the other device - not changing the Transport ID !?")]
        [MemberData(nameof(ChromecastReceiversFilter.GetJblSpeaker), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplicationTwiceWithoutJoining1(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(receiver);
            status = await client.LaunchApplicationAsync("B3419EF5", false);


            // My JBL Device (almost every time - but not always ) makes a new ID here!!!! (The other device - ChromecastAudio DOES NOT!?)
            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetDefaultDevice), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplicationTwiceWithoutJoining2(ChromecastReceiver receiver) {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(receiver);
            status = await client.LaunchApplicationAsync("B3419EF5", false);

            // My ChromecastAudio device keeps the same transport session here!
            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }


        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplicationAThenLaunchApplicationB(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("A9BCCB7C");           //Youtube

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(receiver);
            status = await client.LaunchApplicationAsync("B3419EF5");               //My sample Application

            Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task ConnectToChromecastAndLaunchApplicationOnceAndJoinIt(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5");
            
            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
        }

        //Seems like this isn't really working anymore and just loading a white screen
        [Fact(Skip = "Seems like this isn't really working anymore and just loading a white screen")]
        public async Task ConnectToChromecastAndLaunchWebPage()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(output, "5CB45E5A");

            var req = new WebMessage
            {
                Type = "loc",
                Url = "https://www.google.com/"
            };


            await client.SendAsync(null, "urn:x-cast:com.url.cast", req, "receiver-0");
        }
    }
}
