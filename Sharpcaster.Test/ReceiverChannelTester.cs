using Sharpcaster.Core.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sharpcaster.Test
{
    public class ReceiverChannelTester
    {
        [Fact]
        public async void TestMute()
        {
            var chromecast = await TestHelper.FindChromecast();

            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.GetChannel<ReceiverChannel>().SetMute(true);
            Assert.True(status.Volume.Muted);
        }

        [Fact]
        public async void TestUnMute()
        {
            var chromecast = await TestHelper.FindChromecast();

            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.GetChannel<ReceiverChannel>().SetMute(false);
            Assert.False(status.Volume.Muted);
        }

        [Fact]
        public async void TestVolume()
        {
            var chromecast = await TestHelper.FindChromecast();

            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.GetChannel<ReceiverChannel>().SetVolume(0.1);
            Assert.Equal(status.Volume.Level.Value, 0.1, precision:1);
            
            status = await client.GetChannel<ReceiverChannel>().SetVolume(1.0);
            Assert.Equal(status.Volume.Level.Value, 1.0, precision: 1);
        }

        [Fact]
        public async void TestStoppingApplication()
        {
            var chromecast = await TestHelper.FindChromecast();

            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.LaunchApplicationAsync("B3419EF5");

            status = await client.GetChannel<ReceiverChannel>().StopApplication(status.Applications[0].SessionId);
            Assert.Null(status.Applications);
        }
    }
}
