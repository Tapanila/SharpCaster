# SharpCaster - Modern .NET Chromecast SDK

🚀 **Cross-platform** .NET library for seamless Google Chromecast integration

## ✨ Key Features

- 🔍 **Auto-discovery** via mDNS/Bonjour
- 🎵 **Media Control** - Play, pause, seek, volume
- 📋 **Queue Management** - Playlists with navigation
- ⚡ **Native AOT** support for .NET 9
- 🌐 **Cross-platform** - Windows, macOS, Linux
- 🔄 **Async/Await** throughout
- 📱 **Custom Channels** for specialized apps

## 🎯 Perfect For

- Media streaming applications
- Home automation systems
- Digital signage solutions
- Custom Cast receivers
- IoT projects with audio/video

## 🚀 Quick Start

```csharp
// Discover and connect
var locator = new MdnsChromecastLocator();
var devices = await locator.FindReceiversAsync();
var client = new ChromecastClient();
await client.ConnectChromecast(devices.First());

// Stream media
await client.LaunchApplicationAsync("CC1AD845");
await client.MediaChannel.LoadAsync(new Media 
{
    ContentUrl = "https://example.com/video.mp4",
    ContentType = "video/mp4"
});
```

## 🔧 Compatibility

- **.NET 9** with Native AOT
- **.NET Standard 2.0** for maximum compatibility
- Works with: .NET Framework 4.6.1+, .NET Core 2.0+, Xamarin, Unity

## 📚 Documentation

Full examples, troubleshooting, and API docs at [GitHub](https://github.com/Tapanila/SharpCaster)