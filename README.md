![Icon](https://raw.githubusercontent.com/Tapanila/SharpCaster/master/Assets/sharpcaster-logo-64x64.png)
# SharpCaster

### Currently Supported Platforms
* [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)

[![.NET](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml)

### This project is completed

I won't be adding any new features to this project.
SharpCaster is Chromecast C# SDK any platform support .net standard 2.0.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

https://nuget.org/packages/SharpCaster/

    PM> Install-Package SharpCaster

# Getting started

## Finding chromecast devices from network
```cs
IChromecastLocator locator = new MdnsChromecastLocator();
var chromecasts = await locator.FindReceiversAsync();
//If that does not return devices on desktop then you can use this, Where 192.168.1.2 is your machines local ip
var chromecasts = await locator.FindReceiversAsync("192.168.1.2");
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
_ = await client.GetChannel<IMediaChannel>().LoadAsync(media);
```    

## SharpCaster Demo

![SharpCaster Simple demo](https://raw.githubusercontent.com/tapanila/SharpCaster/master/Assets/SharpCaster.Simple.Demo.gif)
