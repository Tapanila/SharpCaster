using Microsoft.VisualStudio.TestPlatform.Utilities;
using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sharpcaster.Test.helper;
using Xunit.Abstractions;
using Sharpcaster.Channels;

namespace Sharpcaster.Test
{
    public class MultiZoneChannelTester: IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public MultiZoneChannelTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetGoogleCastGroup), MemberType = typeof(ChromecastReceiversFilter))]
        //[MemberData(nameof(CCDevices.GetJblSpeaker), MemberType = typeof(CCDevices))]
        //[MemberData(nameof(CCDevices.GetAny), MemberType = typeof(CCDevices))]
        public async Task TestingMultiZone(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

            //We are going to load video & start playing it
            //Then we are going to change the volume few times

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            int commandsToRun = 5;

            //We are setting up an event listener for multi zone messages
            client.GetChannel<MultiZoneChannel>().StatusChanged += (sender, args) =>
            {
                _autoResetEvent.Set();
            };

            //We are setting up an event listener for multi zone messages
            client.GetChannel<MultiZoneChannel>().DeviceUpdated += (sender, args) =>
            {
                _autoResetEvent.Set();
            };

            var mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            for (int i = 0; i < commandsToRun; i++)
            {
                if (i % 2 == 0)
                {
                    await Task.Delay(1000);
                    await client.GetChannel<ReceiverChannel>().SetVolume(0.2);
                }
                else
                {
                    await Task.Delay(1000);
                    await client.GetChannel<ReceiverChannel>().SetVolume(0.3);
                }
            }

            Assert.True(_autoResetEvent.WaitOne(1000));
        }

    }
}
