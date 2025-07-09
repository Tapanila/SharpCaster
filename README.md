![SharpCaster Logo](https://raw.githubusercontent.com/Tapanila/SharpCaster/master/Assets/sharpcaster-logo-64x64.png)

# SharpCaster

[![.NET Build Status](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Tapanila/SharpCaster/actions/workflows/dotnet.yml)
[![NuGet Status](http://img.shields.io/nuget/v/SharpCaster.svg?style=flat)](https://www.nuget.org/packages/SharpCaster/)

SharpCaster is a cross-platform C# SDK for communicating with Google Chromecast devices. It enables .NET applications to discover, connect, launch apps, and control media playback on Chromecast devices.

---

## Features
- Discover Chromecast devices on your local network
- Connect and launch applications on Chromecast
- Load and control media playback (play, pause, stop, etc.)
- Support for custom Chromecast channels
- Compatible with .NET Standard 2.0 and .NET 9

---

## Supported Platforms
- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)
- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

---

## Installation
Install the NuGet package:
PM> Install-Package SharpCaster
Or via .NET CLI:
dotnet add package SharpCaster
[NuGet Gallery](https://nuget.org/packages/SharpCaster/)

---

## Getting Started

### 1. Discover Chromecast DevicesIChromecastLocator locator = new MdnsChromecastLocator();
var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
var chromecasts = await locator.FindReceiversAsync(source.Token);
### 2. Connect, Launch App, and Load Mediavar chromecast = chromecasts.First();
var client = new ChromecastClient();
await client.ConnectChromecast(chromecast);
await client.LaunchApplicationAsync("B3419EF5"); // Replace with your app ID

var media = new Media
{
    ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
};
await client.MediaChannel.LoadAsync(media);
---

## Demo

![SharpCaster Simple demo](https://raw.githubusercontent.com/tapanila/SharpCaster/master/Assets/SharpCaster.Simple.Demo.gif)

---

## Custom Chromecast Channels

You can add support for custom Chromecast channels by reverse engineering the communication:

1. In Chrome, go to `chrome://net-export/`
2. Select 'Include raw bytes (will include cookies and credentials)'
3. Click 'Start Logging to Disk'
4. Cast from your favorite web app
5. Stop logging and open the log in [netlog-viewer](https://netlog-viewer.appspot.com/)
6. Search for `type:SOCKET` and find familiar JSON data
7. Collect the exchanged JSON
8. Create a new class inheriting from `ChromecastChannel` and implement your logic

---

## Contributing
Contributions, issues, and feature requests are welcome! Feel free to open an issue or submit a pull request.

---

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
