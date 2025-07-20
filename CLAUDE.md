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