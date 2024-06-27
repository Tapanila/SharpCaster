using Sharpcaster.Channels;
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
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);

            status = await client.GetChannel<ReceiverChannel>().SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
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
