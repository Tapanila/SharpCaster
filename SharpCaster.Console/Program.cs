// See https://aka.ms/new-console-template for more information
using Sharpcaster.Interfaces;
using Sharpcaster;
using Sharpcaster.Models.Media;

MdnsChromecastLocator locator = new();
var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
var chromecasts = await locator.FindReceiversAsync(source.Token);

var chromecast = chromecasts.First();
var client = new ChromecastClient();
await client.ConnectChromecast(chromecast);
_ = await client.LaunchApplicationAsync("B3419EF5");

var media = new Media
{
    ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
};
_ = await client.MediaChannel.LoadAsync(media);
