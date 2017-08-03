using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sharpcaster.Test
{
    public class MediaChannelTester
    {
        [Fact]
        public async void TestLoadingMedia()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.LaunchApplicationAsync("B3419EF5");

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            var mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);
        }
    }
}
