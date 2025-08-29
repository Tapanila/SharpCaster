# 🎵 SharpCaster Console Controller

A beautiful, feature-rich console application for controlling Chromecast devices with an intuitive interface powered by [Spectre.Console](https://spectreconsole.net/).

## ✨ Features

- 🔍 **Device Discovery** - Automatically discover Chromecast devices on your network
- 🔗 **Device Connection** - Connect to any discovered Chromecast device  
- 📺 **Media Casting** - Cast video, audio, or images from URLs
- 🎮 **Media Controls** - Full playback control (play, pause, stop, seek, volume)
- 📝 **Queue Management** - Advanced playlist management with shuffle and repeat modes
- 📊 **Device Status** - Real-time device information and running applications
- 🎨 **Beautiful UI** - Rich console interface with colors, tables, and progress indicators
- ⚡ **Interactive** - Intuitive menus and prompts for easy navigation

## 🚀 Quick Start

### Prerequisites

- Chromecast device(s) on the same network
- Network access for device discovery

### Installation

#### Option 1: Homebrew (macOS/Linux)

```bash
# Add the tap
brew tap Tapanila/sharpcaster

# Install SharpCaster
brew install sharpcaster
```

#### Option 2: Chocolatey (Windows)

```powershell
# Install SharpCaster
choco install sharpcaster --pre
```

#### Option 3: Build from Source

1. Clone the repository:
```bash
git clone https://github.com/Tapanila/SharpCaster.git
cd SharpCaster
```

2. Build and run the console application:
```bash
dotnet run --project SharpCaster.Console
```

## 📖 Usage Guide

### Main Menu Navigation

The application presents a beautiful main menu with the following options:

```
┌─────────────────────────────────────────────────────────┐
│                SharpCaster Console Controller           │
└─────────────────────────────────────────────────────────┘

Status: Connected to: Living Room TV

What would you like to do?
🔍 Discover Chromecast devices
🔗 Connect to device
📺 Cast media
🎮 Media controls
📝 Queue management
📊 Device status
❌ Exit
```

### 🔍 Device Discovery

Automatically scan your network for available Chromecast devices:

```bash
# The application will show a spinning indicator while scanning
🔍 Discovering Chromecast devices...
⠋ Scanning network...

# Results are displayed in a beautiful table
✅ Found 3 device(s):
┌───┬─────────────────┬─────────────────┬──────────────────────┬────────┐
│ # │ Device Name     │ Model           │ Address              │ Status │
├───┼─────────────────┼─────────────────┼──────────────────────┼────────┤
│ 1 │ Living Room TV  │ Chromecast      │ http://192.168.1.10  │ Ready  │
│ 2 │ Kitchen Speaker │ Chromecast Audio│ http://192.168.1.11  │ Ready  │
│ 3 │ Bedroom TV      │ Chromecast Ultra│ http://192.168.1.12  │ Ready  │
└───┴─────────────────┴─────────────────┴──────────────────────┴────────┘
```

### 🔗 Device Connection

Select and connect to any discovered device:

```bash
Select a device to connect to:
1. Living Room TV
2. Kitchen Speaker  
3. Bedroom TV

⭐ Connecting to Living Room TV...
✅ Connected to Living Room TV
```

### 📺 Media Casting

Cast different types of media with intelligent content type detection:

#### Casting a Video
```bash
What type of media would you like to cast?
🎬 Video (MP4/WebM/etc)
🎵 Audio (MP3/AAC/etc)
🖼️ Image (JPG/PNG/etc)

Enter media URL: https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4
Enter media title (optional): Big Buck Bunny

⭐ Launching media receiver and loading content...
▸ Launching Default Media Receiver...
▸ Loading media...
✅ Media loaded and playing successfully!
```

#### Casting Audio
```bash
# Example with an audio URL
Enter media URL: https://www.soundjay.com/misc/sounds/bell-ringing-05.wav
Enter media title (optional): Bell Sound

✅ Media loaded and playing successfully!
```

### 🎮 Media Controls

Comprehensive media control with real-time feedback:

```bash
Media Controls:
▶️ Play
⏸️ Pause
⏹️ Stop
⏭️ Seek
🔊 Set volume
📊 Get media status
🔙 Back to main menu
```

#### Media Status Display
```bash
📊 Media Status:
┌─────────────┬────────────────────┐
│ Property    │ Value              │
├─────────────┼────────────────────┤
│ State       │ PLAYING            │
│ Current Time│ 45.2s              │
│ Duration    │ 596.5s             │
│ Title       │ Big Buck Bunny     │
│ Progress    │ 7.6%               │
└─────────────┴────────────────────┘
```

#### Volume Control
```bash
Enter volume (0.0 - 1.0): 0.7
🔊 Setting volume to 70%...
🔊 Volume set to 70%
```

#### Seeking
```bash
Enter seek time in seconds: 120
⏭️ Seeking to 120s...
⏭️ Seeked to 120.0s
```

### 📝 Queue Management

Advanced playlist management with multiple tracks:

#### Loading a Queue
```bash
How many tracks do you want to add? 3

Track 1 of 3:
Enter URL for track 1: https://example.com/song1.mp3
Enter title for track 1 (optional): Awesome Song #1

Track 2 of 3:
Enter URL for track 2: https://example.com/song2.mp3
Enter title for track 2 (optional): Great Track #2

Track 3 of 3:
Enter URL for track 3: https://example.com/song3.mp3
Enter title for track 3 (optional): Amazing Tune #3

🎈 Loading queue...
▸ Launching Default Media Receiver...
▸ Loading queue with 3 items...
✅ Queue loaded with 3 items
```

#### Queue Controls
```bash
Queue Management:
📝 Load queue from URLs
⏭️ Next track
⏮️ Previous track
🔀 Toggle shuffle
🔁 Set repeat mode
📋 Get queue items
🔙 Back to main menu
```

#### Shuffle Control
```bash
Enable shuffle? (y/n) y
🔀 Enabling shuffle...
🔀 Shuffle enabled
```

#### Repeat Mode Selection
```bash
Select repeat mode:
🚫 Off
🔁 Repeat All
🔂 Repeat Single

🔁 Setting repeat mode to ALL...
🔁 Repeat mode set to All
```

#### Queue Status
```bash
📋 Queue contains 3 items:
┌─────────┐
│ Item ID │
├─────────┤
│ 1       │
│ 2       │
│ 3       │
└─────────┘
```

### 📊 Device Status

View detailed device information in a beautiful panel:

```bash
┌──────────────────────────────────────────────────────┐
│                📱 Device Status - Living Room TV      │
├──────────────────────────────────────────────────────┤
│ ┌─────────────────────┬──────────────────────────────┐ │
│ │ Property            │ Value                        │ │
│ ├─────────────────────┼──────────────────────────────┤ │
│ │ Volume Level        │ 70%                          │ │
│ │ Muted               │ No                           │ │
│ │ Applications        │ 1                            │ │
│ │                     │                              │ │
│ │ Running Applications:│                             │ │
│ │   • Default Media   │ Now Casting                  │ │
│ │     Receiver        │                              │ │
│ └─────────────────────┴──────────────────────────────┘ │
└──────────────────────────────────────────────────────┘
```

## 🛠️ Advanced Usage

### Supported Media Formats

The application automatically handles content type detection based on your selection:

- **Video**: MP4, WebM, AVI, MKV, and other common video formats
- **Audio**: MP3, AAC, WAV, FLAC, OGG, and other audio formats  
- **Images**: JPG, PNG, GIF, WebP, and other image formats

### Network Requirements

- Chromecast devices must be on the same network as your computer
- mDNS/Bonjour must be enabled for device discovery
- Firewall may need to allow the application for network scanning

### Command Line Options

```bash
# Run the application
dotnet run --project SharpCaster.Console

# Build and run in release mode
dotnet run --project SharpCaster.Console --configuration Release

# Run with specific verbosity
dotnet run --project SharpCaster.Console --verbosity detailed
```


## 🔧 Troubleshooting

### Common Issues

1. **No devices found**
   - Ensure Chromecast devices are powered on and connected to the same network
   - Check that your computer can access the network (try pinging a known device)
   - Verify firewall settings allow network discovery
   - Try increasing the discovery timeout by running discovery multiple times

2. **Connection failed**
   - Device may be busy with another casting session
   - Try restarting the Chromecast device
   - Ensure the device is not in guest mode
   - Check if firewall is blocking the connection
   - The application now provides specific troubleshooting tips when connection fails

3. **Connection lost during operation**
   - The application automatically detects connection loss
   - Look for warning messages about lost connections
   - Try reconnecting to the device from the main menu
   - Check your network stability

4. **Media won't load**
   - Verify the media URL is accessible from your network
   - Check that the media format is supported by Chromecast
   - Ensure the URL serves the media with appropriate MIME types
   - Try a different media URL to test connectivity

5. **Controls not working**
   - The application will automatically check connection before allowing control operations
   - Make sure you're connected to a device first
   - Verify media is currently loaded on the device
   - Some controls may not be available depending on the media type

6. **Emojis not displaying properly**
   - The application now automatically sets UTF-8 encoding
   - If emojis still don't display, your terminal may not support Unicode
   - Try using Windows Terminal, PowerShell, or a modern terminal emulator

### Debug Information

To enable detailed logging, modify the log level in `Program.cs`:

```csharp
.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
```

## 🤝 Contributing

We welcome contributions! Please feel free to submit pull requests, report bugs, or suggest new features.

### Development Setup

1. Clone the repository
2. Ensure you have .NET 9.0 SDK installed
3. Build the solution: `dotnet build`
4. Run tests: `dotnet test`

## 📄 License

This project is licensed under the MIT License - see the main project LICENSE file for details.

## 🙏 Acknowledgments

- UI powered by [Spectre.Console](https://spectreconsole.net/)

---

**Happy Casting! 🎉**

For more information about the SharpCaster library itself, see the [main project README](../README.md).