![Icon](https://raw.githubusercontent.com/Tapanila/SharpCaster/master/Assets/sharpcaster-logo-64x64.png)
# SharpCaster

### Currently Supported Platforms
* [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)

[![.NET](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml)

SharpCaster is Chromecast C# SDK any platform support .net standard 2.0.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

https://nuget.org/packages/SharpCaster/

    PM> Install-Package SharpCaster

# Getting started

## Finding chromecast devices from network
```cs
IChromecastLocator locator = new MdnsChromecastLocator();
var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
var chromecasts = await locator.FindReceiversAsync(source.Token);
```
## Connecting to chromecast device, launch application and load media
```cs
var chromecast = chromecasts.First();
var client = new ChromecastClient();
await client.ConnectChromecast(chromecast);
_ = await client.LaunchApplicationAsync("B3419EF5");

var media = new Media
{
	ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
};
_ = await client.MediaChannel.LoadAsync(media);
```    

## SharpCaster Demo

![SharpCaster Simple demo](https://raw.githubusercontent.com/tapanila/SharpCaster/master/Assets/SharpCaster.Simple.Demo.gif)

## Adding support for custom chromecast channels

 * In Chrome, go to `chrome://net-export/`
 * Select 'Include raw bytes (will include cookies and credentials)'
 * Click 'Start Logging to Disk'
 * Open a new tab, browse to your favorite application on the web that has Chromecast support and start casting.
 * Go back to the tab that is capturing events and click on stop.
 * Open https://netlog-viewer.appspot.com/ and select your event log file.
 * Browse to https://netlog-viewer.appspot.com/#events&q=type:SOCKET, and find the socket that has familiar JSON data.
 * Go through the results and collect the JSON that is exchanged.
 * Now you can create a new class that inherits from ChromecastChannel and implement the logic to send and receive messages.
