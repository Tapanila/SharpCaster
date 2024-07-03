﻿using Sharpcaster.Test.customChannel;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastApplicationTester
    {
        private ITestOutputHelper output;
        public ChromecastApplicationTester(ITestOutputHelper outputHelper) {
            output = outputHelper;
        }

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
            var client = await TestHelper.CreateAndConnectClient(output);

            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;
            await client.DisconnectAsync();

            status = await client.ConnectChromecast(TestHelper.CurrentReceiver);
            status = await client.LaunchApplicationAsync("B3419EF5", false);

            // ??????
            // My JBL Device (almost every time - but not always ) makes a new ID here!!!! (The other device - ChromecastAudio DOES NOT!?)
            if (TestHelper.CurrentReceiver.Model.Contains("JBL")) {
                Assert.NotEqual(firstLaunchTransportId, status.Applications[0].TransportId);
            } else {
                Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
            }

            
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
            var client = await TestHelper.CreateAndConnectClient(output);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            var firstLaunchTransportId = status.Applications[0].TransportId;

            status = await client.LaunchApplicationAsync("B3419EF5");

            
            Assert.Equal(firstLaunchTransportId, status.Applications[0].TransportId);
            
        }

        //Seems like this isn't really working anymore and just loading a white screen
        [Fact]
        public async Task ConnectToChromecastAndLaunchWebPage()
        {
            var client = await TestHelper.CreateConnectAndLoadAppClient(output, "5CB45E5A");

            var req = new WebMessage
            {
                Type = "loc",
                Url = "https://www.google.com/"
            };


            await client.SendAsync("urn:x-cast:com.url.cast", req, "receiver-0");
        }
    }
}
