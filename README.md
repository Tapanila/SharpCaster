# SharpCaster

[![Build status](https://ci.appveyor.com/api/projects/status/myew8u24ry7dbdm0?svg=true)](https://ci.appveyor.com/project/tapanila/sharpcaster)

### This project is on beta stage

SharpCaster is Chromecast C# SDK for Windows, Windows Phone, .NET 4.5.1, Xamarin.iOS and Xamarin.Android platforms.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

https://nuget.org/packages/SharpCaster/

    PM> Install-Package SharpCaster

# Getting started

## Finding chromecast devices from network
```cs
ObservableCollection<Chromecast> chromecasts = await ChromecastService.Current.StartLocatingDevices();
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

# Notes

Heavily based on [NCast](https://github.com/jeremychild/NCast)