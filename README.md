![Icon](https://raw.githubusercontent.com/Tapanila/SharpCaster/master/Assets/sharpcaster-logo-64x64.png)
# SharpCaster

### Currently Supported Platforms
* .Net Framework 4.5.1
* Xamarin.iOS Unified
* Xamarin.Android
* UWP 10+ (Windows 10 Universal Programs)

[![Build status](https://ci.appveyor.com/api/projects/status/myew8u24ry7dbdm0/branch/master?svg=true)](https://ci.appveyor.com/project/tapanila/sharpcaster)

### This project is on beta stage (There might be breaking changes and supported platforms might change)

SharpCaster is Chromecast C# SDK for Windows, Windows Phone, .NET 4.5.1, Xamarin.iOS and Xamarin.Android platforms.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

https://nuget.org/packages/SharpCaster/

    PM> Install-Package SharpCaster

# Getting started

## Finding chromecast devices from network
```cs
ObservableCollection<Chromecast> chromecasts = await ChromecastService.Current.StartLocatingDevices();
//If that does not return devices on desktop then you can use this, Where 192.168.1.2 is your machines local ip
ObservableCollection<Chromecast> chromecasts = await ChromecastService.Current.StartLocatingDevices("192.168.1.2);
```
## Connecting to chromecast device, launch application and load media
```cs
var chromecast = chromecasts.First();
SharpCasterDemoController _controller;
ChromecastService.Current.ChromeCastClient.ConnectedChanged += async delegate { if (_controller == null)_controller = await ChromecastService.Current.ChromeCastClient.LaunchSharpCaster(); };
ChromecastService.Current.ChromeCastClient.ApplicationStarted += 
async delegate { 
	while (_controller == null)
	{
		await Task.Delay(500);
	}

	await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4");
};
ChromecastService.Current.ConnectToChromecast(chromecast);
```    

## SharpCaster Simple

![SharpCaster Simple demo](https://raw.githubusercontent.com/tapanila/SharpCaster/master/Assets/SharpCaster.Simple.Demo.gif)

## Contributing
Contributing is encouraged! Please submit pull requests, open issues etc. However, to ensure we end up with a good result and to make my life a little easier, could I please request that;

* All changes be made in a feature branch, not in master, and please don't submit PR's directly against master.

Thanks! I look forward to merge your contribution.
