# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

### Building the Project
```bash
# Restore dependencies
dotnet restore SharpCaster.sln

# Build all projects
dotnet build SharpCaster.sln

# Build specific project (main library)
dotnet build Sharpcaster/Sharpcaster.csproj

# Build in Release mode
dotnet build --configuration Release Sharpcaster/Sharpcaster.csproj
```

### Running Tests
```bash
# Run all tests
dotnet test SharpCaster.sln

# Run tests for specific project
dotnet test Sharpcaster.Test/Sharpcaster.Test.csproj

# Run AOT compatibility tests
dotnet test Sharpcaster.Test.Aot/Sharpcaster.Test.Aot.csproj

# Run tests with live output
dotnet test --logger "console;verbosity=detailed"
```

### Packaging
```bash
# Create NuGet package
dotnet pack --configuration Release Sharpcaster/Sharpcaster.csproj
```

## Project Architecture

### Core Components

**ChromecastClient** (`Sharpcaster/ChromeCastClient.cs`) - Main entry point for Chromecast communication. Manages TCP connection, SSL stream, and coordinates all channels.

**Channel-based Architecture** - Communication is organized through specialized channels in `Sharpcaster/Channels/`:
- `ChromecastChannel` - Base class for all channels
- `MediaChannel` - Media playback control (play, pause, stop, seek, queue management)
- `ReceiverChannel` - App launching and receiver status
- `ConnectionChannel` - Connection management
- `HeartbeatChannel` - Keep-alive functionality
- `MultiZoneChannel` - Multi-room audio support
- `SpotifyChannel` - Spotify-specific integration

**Message System** - Type-safe message handling in `Sharpcaster/Messages/`:
- Protocol Buffer messages for low-level communication
- JSON messages for application-level communication
- Strongly-typed message classes with System.Text.Json serialization

### Key Patterns

**Dependency Injection** - Uses Microsoft.Extensions.DependencyInjection for channel management and logging.

**Async/Await** - All operations are async with proper cancellation token support.

**Message Routing** - Messages are routed by namespace and request ID using `SharpCasterTaskCompletionSource` for request-response correlation.

**Protobuf Integration** - Uses Google.Protobuf for efficient binary communication with Chromecast devices.

### Target Frameworks
- .NET Standard 2.0 (broad compatibility)
- .NET 9 (latest features, AOT compatibility)

### Testing Strategy
- Main test suite uses xUnit with Moq for mocking
- Separate AOT test project for Native AOT compatibility verification
- Integration tests require actual Chromecast devices on network
- Test environment always has Chromecast devices connected and available
- **Important**: Tests can take up to 10 minutes to complete as they perform real network operations with Chromecast devices

### Key Dependencies
- `Google.Protobuf` - Protocol buffer serialization for Cast protocol communication
- `System.Text.Json` - JSON serialization (replaced Newtonsoft.Json)
- `Zeroconf` - mDNS device discovery for finding Chromecast devices on network
- `Microsoft.Extensions.DependencyInjection` - Dependency injection container
- `Microsoft.Extensions.Logging.Abstractions` - Logging framework

## MediaChannel Enhanced Features

### Advanced Media Control Methods
The MediaChannel now supports comprehensive Google Cast SDK functionality:

#### Basic Playback Control
- `PlayAsync()` - Play media
- `PauseAsync()` - Pause media
- `StopAsync()` - Stop media
- `SeekAsync(double seconds)` - Seek to specific time
- `SetVolumeAsync(double level)` - Set media stream volume level
- `SetMuteAsync(bool muted)` - Set media stream mute state

#### Queue Management
- `QueueLoadAsync(QueueItem[] items, RepeatModeType repeatMode, int startIndex)` - Load queue
- `QueueNextAsync()` - Skip to next item
- `QueuePrevAsync()` - Go to previous item
- `QueueShuffleAsync(bool shuffle)` - Enable/disable shuffle
- `QueueSetRepeatModeAsync(RepeatModeType repeatMode)` - Set repeat mode
- `QueueInsertAsync(QueueItem[] items, int? insertBefore)` - Insert items
- `QueueRemoveAsync(int[] itemIds)` - Remove items
- `QueueReorderAsync(int[] itemIds, int? insertBefore)` - Reorder items
- `QueueUpdateAsync(QueueItem[] items)` - Update items
- `QueueGetItemsAsync(int[] ids)` - Get queue items
- `QueueGetItemIdsAsync()` - Get all queue item IDs

#### Advanced Control Methods
- `SendUserActionAsync(UserAction userAction)` - Send user interactions
- `EditTracksAsync(EditTracksInfoRequest editTracksInfo)` - Edit track information
- `StreamTransferAsync(object transferRequest)` - Transfer stream to another device
- `SetPlaybackRateAsync(double playbackRate)` - Change playback speed

### MediaCommand Flags Enum
`MediaCommand` is a comprehensive flags enum supporting all Google Cast SDK commands:
- Basic commands: `PAUSE`, `SEEK`, `STREAM_VOLUME`, `STREAM_MUTE`
- Queue commands: `QUEUE_NEXT`, `QUEUE_PREV`, `QUEUE_SHUFFLE`, `QUEUE_REPEAT_ALL`, `QUEUE_REPEAT_ONE`
- Advanced commands: `EDIT_TRACKS`, `PLAYBACK_RATE`, `STREAM_TRANSFER`
- Social commands: `LIKE`, `DISLIKE`, `FOLLOW`, `UNFOLLOW`

#### Extension Methods
- `GetIndividualCommands()` - Extract individual commands from combined flags
- `SupportsCommand(MediaCommand command)` - Check if specific command is supported
- `GetCommandNames()` - Get human-readable command names

### Error Handling and Debugging
- All MediaChannel methods return nullable `MediaStatus?` for error handling
- Use `MediaStatus.SupportedMediaCommands` to check available commands
- Monitor `MediaChannel.StatusChanged` event for real-time updates

## SharpCaster Console Application

The console application (`SharpCaster.Console`) provides both interactive and command-line interfaces for controlling Chromecast devices. The executable is distributed as `sharpcaster` for end users.

### Running the Console Application
```bash
# Build console application
dotnet build SharpCaster.Console/SharpCaster.Console.csproj

# Run in interactive mode
dotnet run --project SharpCaster.Console/SharpCaster.Console.csproj

# Run with command line arguments
dotnet run --project SharpCaster.Console/SharpCaster.Console.csproj -- <args>
```

### Command Line Usage

#### Basic Commands
- `help` - Show usage information
- `list` - List available Chromecast devices on network
- `version` - Show application version information

#### Media Control Commands
- `play <url>` - Cast and play media from URL
- `pause` - Pause current media
- `stop` - Stop current media  
- `volume <0.0-1.0>` - Set device volume level (ReceiverChannel)
- `media-volume <0.0-1.0>` - Set media stream volume level (MediaChannel)
- `seek <seconds>` - Seek to specific time
- `status` - Show current device and media status

#### Device Connection Options
- **Discovery Mode (Default)**: `sharpcaster <device-name> <command>`
  - Uses mDNS discovery to find devices on network
  - Supports partial name matching (case-insensitive)
  - Example: `sharpcaster "Living Room TV" play "https://example.com/video.mp4"`

- **Direct IP Mode**: `sharpcaster --ip <ip-address> <command>`
  - Connects directly to device IP address, bypassing discovery
  - Faster connection when IP is known
  - Useful for automation and scripting
  - Example: `sharpcaster --ip 192.168.1.100 play "https://example.com/video.mp4"`

#### Additional Options
- `--title <title>` or `-t <title>` - Set custom media title
- `--ip <ip-address>` or `-i <ip-address>` - Connect directly to IP (skips discovery)

#### Examples
```bash
# Interactive mode
sharpcaster

# List devices
sharpcaster list

# Play media on discovered device
sharpcaster "Office TV" play "https://example.com/video.mp4" --title "My Video"

# Connect directly to IP and play media
sharpcaster --ip 192.168.1.100 play "https://example.com/video.mp4"

# Control playback
sharpcaster "Kitchen Speaker" pause
sharpcaster --ip 192.168.1.100 volume 0.7
sharpcaster "Living Room TV" media-volume 0.5
sharpcaster "Bedroom TV" seek 120

# Check status
sharpcaster "Living Room TV" status
```

### Console Application Architecture

#### Key Components
- **Program.cs** - Entry point with dependency injection setup
- **CommandLineArgs.cs** - Command line argument parsing and validation
- **CommandExecutor.cs** - Executes commands in non-interactive mode
- **DeviceService.cs** - Device discovery and connection management
- **ApplicationFlows.cs** - Interactive mode user interface flows
- **Controllers/** - Media and queue control logic
- **UI/UIHelper.cs** - Console UI utilities using Spectre.Console

#### Direct IP Connection Feature
When `--ip` option is used:
1. IP address validation using `System.Net.IPAddress.TryParse()`
2. Creates `ChromecastReceiver` object directly with IP:8009
3. Skips mDNS discovery entirely for faster connection
4. Handles connection failures with clear error messages
5. Works with all media control commands