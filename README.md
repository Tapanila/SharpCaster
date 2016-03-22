# SharpCaster

[![Build status](https://ci.appveyor.com/api/projects/status/myew8u24ry7dbdm0?svg=true)](https://ci.appveyor.com/project/tapanila/sharpcaster)

### This project is on beta stage

SharpCaster is Chromecast C# SDK for Windows and Windows Phone platforms.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

https://nuget.org/packages/SharpCaster/

    PM> Install-Package SharpCaster

# Getting started

## Finding chromecast devices from network
```cs
DeviceLocator deviceLocator = new DeviceLocator();
var cancellationTokenSource = new CancellationTokenSource();
cancellationTokenSource.CancelAfter(5000);
List<Chromecast> chromecasts = await deviceLocator.LocateDevicesAsync(cancellationTokenSource.Token);
```
## Connecting to chromecast device, launch application and load media
```cs
var chromecast = chromecasts.First();
var client = new ChromeCastClient();
client.Connected += async delegate { await client.LaunchApplication("B3419EF5"); };
client.ApplicationStarted += async delegate { await client.LoadMedia("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd"); };
client.ConnectChromecast(chromecast.DeviceUri);
```    

## SharpCaster Simple

![SharpCaster Simple demo](https://raw.githubusercontent.com/tapanila/SharpCaster/master/Assets/SharpCaster.Simple.Demo.gif)

# Notes

Heavily based on [NCast](https://github.com/jeremychild/NCast)
