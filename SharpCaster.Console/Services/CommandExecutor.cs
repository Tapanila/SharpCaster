using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Messages.Web;
using Sharpcaster.Models.Media;
using SharpCaster.Console.Models;
using SharpCaster.Console.UI;
using Spectre.Console;
using System.Text.Json;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

namespace SharpCaster.Console.Services;

public class CommandExecutor
{
    private readonly ApplicationState _state;
    private readonly DeviceService _deviceService;
    private readonly MemoryLogService _memoryLogService;
    private readonly ILogger<Sharpcaster.ChromecastClient> _chromecastLogger;
    private readonly UIHelper _ui;

    public CommandExecutor(ApplicationState state, DeviceService deviceService, MemoryLogService memoryLogService, ILogger<Sharpcaster.ChromecastClient> chromecastLogger, UIHelper ui, Sharpcaster.MdnsChromecastLocator locator)
    {
        _state = state;
        _deviceService = deviceService;
        _memoryLogService = memoryLogService;
        _chromecastLogger = chromecastLogger;
        _ui = ui;
        
        // Ensure locator is initialized
        if (_state.Locator == null)
        {
            _state.Locator = locator;
        }
    }

    public async Task<int> ExecuteCommandAsync(CommandLineArgs args)
    {
        try
        {
            if (args.ShowHelp)
            {
                CommandLineParser.ShowHelp();
                return 0;
            }

            if (args.ShowVersion)
            {
                CommandLineParser.ShowVersion();
                return 0;
            }

            if (args.ShowDevices)
            {
                var listResult = await ListDevicesAsync();
                
                // Show logs if requested
                if (args.ShowLogs)
                {
                    System.Console.WriteLine();
                    await ShowLogsAsync();
                }
                
                return listResult;
            }

            if (string.IsNullOrEmpty(args.Command))
            {
                System.Console.WriteLine("Error: Command is required for command-line mode.");
                System.Console.WriteLine("Use 'sharpcaster help' for usage information.");
                return 1;
            }

            Sharpcaster.Models.ChromecastReceiver device;

            // Handle direct IP connection
            if (!string.IsNullOrEmpty(args.DeviceIpAddress))
            {
                if (!System.Net.IPAddress.TryParse(args.DeviceIpAddress, out _))
                {
                    System.Console.WriteLine($"Error: Invalid IP address '{args.DeviceIpAddress}'.");
                    return 1;
                }

                device = CreateDeviceFromIpAddress(args.DeviceIpAddress);
                System.Console.WriteLine($"Connecting directly to {args.DeviceIpAddress}...");
            }
            else
            {
                // Discover devices and find by name
                if (string.IsNullOrEmpty(args.DeviceName))
                {
                    System.Console.WriteLine("Error: Either device name or IP address is required for command-line mode.");
                    System.Console.WriteLine("Use 'sharpcaster help' for usage information.");
                    return 1;
                }

                await DiscoverDevicesQuietAsync();
                
                if (_state.Devices.Count == 0)
                {
                    System.Console.WriteLine("Error: No Chromecast devices found on the network.");
                    return 1;
                }

                // Find matching device
                device = FindDevice(args.DeviceName);
                if (device == null)
                {
                    System.Console.WriteLine($"Error: Device '{args.DeviceName}' not found.");
                    System.Console.WriteLine("Available devices:");
                    foreach (var d in _state.Devices)
                    {
                        System.Console.WriteLine($"  - {d.Name} ({d.Model})");
                    }
                    return 1;
                }
            }

            // Connect to device
            _state.SelectedDevice = device;
            await ConnectToDeviceQuietAsync();
            
            if (!_state.IsConnected)
            {
                System.Console.WriteLine($"Error: Failed to connect to device '{device.Name}'.");
                return 1;
            }

            var connectionInfo = !string.IsNullOrEmpty(args.DeviceIpAddress) 
                ? $"device at {args.DeviceIpAddress}" 
                : device.Name;
            System.Console.WriteLine($"Connected to {connectionInfo}");

            // Execute command
            var result = await ExecuteSpecificCommandAsync(args);
            
            // Show logs if requested
            if (args.ShowLogs)
            {
                System.Console.WriteLine();
                await ShowLogsAsync();
            }
            
            return result;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
            
            // Show logs if requested even on error
            if (args.ShowLogs)
            {
                System.Console.WriteLine();
                await ShowLogsAsync();
            }
            
            return 1;
        }
        finally
        {
            // Cleanup
            _state.Client?.Dispose();
            _state.Locator?.Dispose();
        }
    }

    private async Task<int> ListDevicesAsync()
    {
        System.Console.WriteLine("Discovering Chromecast devices...");
        await DiscoverDevicesQuietAsync();
        
        if (_state.Devices.Count == 0)
        {
            System.Console.WriteLine("No Chromecast devices found on the network.");
            return 1;
        }

        System.Console.WriteLine($"\nFound {_state.Devices.Count} device(s):");
        foreach (var device in _state.Devices)
        {
            System.Console.WriteLine($"  - {device.Name} ({device.Model}) at {device.DeviceUri?.Host}");
        }
        
        return 0;
    }

    private async Task DiscoverDevicesQuietAsync()
    {
        try
        {
            _state.Devices.Clear();
            var devices = await _state.Locator!.FindReceiversAsync(TimeSpan.FromSeconds(8));
            _state.Devices.AddRange(devices);
        }
        catch (Exception ex)
        {
            throw new Exception($"Device discovery failed: {ex.Message}");
        }
    }

    private async Task ConnectToDeviceQuietAsync()
    {
        if (_state.SelectedDevice == null) return;
        
        try
        {
            _state.Client?.Dispose();
            _state.Client = new Sharpcaster.ChromecastClient(_chromecastLogger);
            
            await _state.Client.ConnectChromecast(_state.SelectedDevice);
            await Task.Delay(1000); // Give connection time to stabilize
            
            // Verify the connection
            var status = _state.Client.ReceiverChannel?.ReceiverStatus;
            if (status == null)
            {
                throw new Exception("Connection established but device is not responding");
            }
            
            _state.IsConnected = true;
            _state.LastConnectionCheck = DateTime.Now;
        }
        catch (Exception ex)
        {
            _state.Client?.Dispose();
            _state.Client = null;
            _state.IsConnected = false;
            throw new Exception($"Connection failed: {ex.Message}");
        }
    }

    private Sharpcaster.Models.ChromecastReceiver? FindDevice(string deviceName)
    {
        // Try exact match first
        var exactMatch = _state.Devices.FirstOrDefault(d => 
            string.Equals(d.Name, deviceName, StringComparison.OrdinalIgnoreCase));
        if (exactMatch != null) return exactMatch;

        // Try partial match
        var partialMatch = _state.Devices.FirstOrDefault(d => 
            d.Name.Contains(deviceName, StringComparison.OrdinalIgnoreCase));
        return partialMatch;
    }

    private Sharpcaster.Models.ChromecastReceiver CreateDeviceFromIpAddress(string ipAddress)
    {
        return new Sharpcaster.Models.ChromecastReceiver
        {
            DeviceUri = new Uri($"https://{ipAddress}:8009"),
            Name = $"Chromecast at {ipAddress}",
            Port = 8009,
            Model = "Unknown",
            Version = "Unknown",
            Status = "0",
            ExtraInformation = new Dictionary<string, string>()
        };
    }

    private async Task<int> ExecuteSpecificCommandAsync(CommandLineArgs args)
    {
        try
        {
            switch (args.Command?.ToLowerInvariant())
            {
                case "play":
                    if (string.IsNullOrEmpty(args.MediaUrl))
                    {
                        System.Console.WriteLine("Error: Media URL is required for play command.");
                        return 1;
                    }
                    return await PlayMediaAsync(args.MediaUrl, args.MediaTitle);
                    
                case "pause":
                    return await PauseMediaAsync();
                    
                case "stop":
                    return await StopMediaAsync();
                    
                case "stop-app":
                    return await StopApplicationAsync();
                    
                case "volume":
                    if (!args.Volume.HasValue)
                    {
                        System.Console.WriteLine($"Error: Volume value (0.0-1.0) is required for volume command. Parsed volume: {args.Volume}");
                        return 1;
                    }
                    return await SetVolumeAsync(args.Volume.Value);
                    
                case "media-volume":
                    if (!args.Volume.HasValue)
                    {
                        System.Console.WriteLine($"Error: Volume value (0.0-1.0) is required for media-volume command. Parsed volume: {args.Volume}");
                        return 1;
                    }
                    return await SetMediaVolumeAsync(args.Volume.Value);
                    
                case "seek":
                    if (!args.SeekTime.HasValue)
                    {
                        System.Console.WriteLine("Error: Seek time in seconds is required for seek command.");
                        return 1;
                    }
                    return await SeekAsync(args.SeekTime.Value);
                    
                case "status":
                    return await ShowStatusAsync();
                    
                case "website":
                    if (string.IsNullOrEmpty(args.MediaUrl))
                    {
                        System.Console.WriteLine("Error: Website URL is required for website command.");
                        return 1;
                    }
                    return await StartWebsiteAsync(args.MediaUrl);
                    
                default:
                    System.Console.WriteLine($"Error: Unknown command '{args.Command}'.");
                    System.Console.WriteLine("Use 'sharpcaster help' for available commands.");
                    return 1;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Command execution failed: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> PlayMediaAsync(string url, string? title)
    {
        try
        {
            System.Console.WriteLine($"Casting media: {url}");

            var mediaType = DetectMediaType(url);
            var media = new Media
            {
                ContentId = url,
                ContentType = mediaType,
                StreamType = StreamType.Buffered,
                Metadata = new MediaMetadata
                {
                    MetadataType = GetMetadataType(mediaType),
                    Title = title ?? "Cast Media"
                }
            };

            var status = await _state.Client.MediaChannel.LoadAsync(media);
            if (status == null)
            {
                System.Console.WriteLine("Failed to load media - no status returned");
                return 1;
            }

            System.Console.WriteLine($"Successfully started casting: {title ?? "media"}");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to cast media: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> PauseMediaAsync()
    {
        try
        {
            var status = await _state.Client!.MediaChannel.PauseAsync();
            if (status == null)
            {
                System.Console.WriteLine("Failed to pause media");
                return 1;
            }
            System.Console.WriteLine("Media paused");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to pause: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> StopMediaAsync()
    {
        try
        {
            await _state.Client!.MediaChannel.StopAsync();
            System.Console.WriteLine("Media stopped");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to stop: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> StopApplicationAsync()
    {
        try
        {
            var receiverStatus = _state.Client!.ReceiverChannel.ReceiverStatus;
            if (receiverStatus?.Applications?.Any() == true)
            {
                var app = receiverStatus.Applications.First();
                await _state.Client.ReceiverChannel.StopApplication();
                System.Console.WriteLine($"Application '{app.DisplayName}' stopped");
            }
            else
            {
                System.Console.WriteLine("No applications running");
            }
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to stop application: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> SetVolumeAsync(double volume)
    {
        try
        {
            if (volume < 0 || volume > 1)
            {
                System.Console.WriteLine("Volume must be between 0.0 and 1.0");
                return 1;
            }
            
            await _state.Client!.ReceiverChannel.SetVolume(volume);
            System.Console.WriteLine($"Volume set to {volume:P0}");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to set volume: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> SetMediaVolumeAsync(double volume)
    {
        try
        {
            if (volume < 0 || volume > 1)
            {
                System.Console.WriteLine("Media volume must be between 0.0 and 1.0");
                return 1;
            }
            
            var status = await _state.Client!.MediaChannel.SetVolumeAsync(volume);
            if (status == null)
            {
                System.Console.WriteLine("Failed to set media volume - no status returned");
                return 1;
            }
            System.Console.WriteLine($"Media volume set to {volume:P0}");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to set media volume: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> SeekAsync(double seekTime)
    {
        try
        {
            await _state.Client!.MediaChannel.SeekAsync(seekTime);
            System.Console.WriteLine($"Seeked to {seekTime:F1} seconds");
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to seek: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> ShowStatusAsync()
    {
        try
        {
            var mediaStatus = await _state.Client!.MediaChannel.GetMediaStatusAsync();
            var receiverStatus = _state.Client.ReceiverChannel.ReceiverStatus;
            
            System.Console.WriteLine($"\nDevice: {_state.SelectedDevice?.Name}");
            System.Console.WriteLine($"Volume: {receiverStatus?.Volume?.Level:P0} (Muted: {receiverStatus?.Volume?.Muted})");
            
            if (mediaStatus != null)
            {
                System.Console.WriteLine($"Media State: {mediaStatus.PlayerState}");
                System.Console.WriteLine($"Title: {mediaStatus.Media?.Metadata?.Title ?? "Unknown"}");
                System.Console.WriteLine($"Current Time: {mediaStatus.CurrentTime:F1}s");
                if (mediaStatus.Media?.Duration > 0)
                {
                    System.Console.WriteLine($"Duration: {mediaStatus.Media.Duration:F1}s");
                    var progress = (mediaStatus.CurrentTime / mediaStatus.Media.Duration.Value) * 100;
                    System.Console.WriteLine($"Progress: {progress:F1}%");
                }
            }
            else
            {
                System.Console.WriteLine("No media currently playing");
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to get status: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> StartWebsiteAsync(string url)
    {
        try
        {
            // Validate URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                System.Console.WriteLine("Error: Invalid website URL. Must be a valid http or https URL.");
                return 1;
            }

            

            try
            {
                const string dashboardReceiver = "F7FD2183";
                
                System.Console.WriteLine("Launching Dashboard Receiver...");
                await _state.Client!.LaunchApplicationAsync(dashboardReceiver, false);


                var req = new WebMessage
                {
                    Url = url,
                    Type = "load",
                    SessionId = _state.Client.ChromecastStatus.Application.SessionId
                };

                var requestPayload = JsonSerializer.Serialize(req, SharpcasteSerializationContext.Default.WebMessage);

                await _state.Client.SendAsync(_chromecastLogger, "urn:x-cast:com.boombatower.chromecast-dashboard", requestPayload, _state.Client.ChromecastStatus.Application.SessionId);

                System.Console.WriteLine($"Successfully opened website on Chromecast");
                return 0;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine($"Website loading failed: {ex.Message}");
                return 1;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to open website: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> ShowLogsAsync()
    {
        try
        {
            var logs = _memoryLogService.GetRecentLogs(20);
            
            if (logs.Count == 0)
            {
                System.Console.WriteLine("No logs available.");
                return 0;
            }

            System.Console.WriteLine($"\nShowing last {logs.Count} log entries:");
            System.Console.WriteLine(new string('-', 80));
            
            foreach (var log in logs)
            {
                var timeStr = log.Timestamp.ToString("HH:mm:ss.fff");
                var levelStr = log.GetLevelDisplay();
                var categoryStr = log.Category.Length > 25 ? log.Category.Substring(0, 22) + "..." : log.Category;
                
                System.Console.WriteLine($"{timeStr} [{levelStr}] {categoryStr}: {log.Message}");
                
                if (log.Exception != null)
                {
                    System.Console.WriteLine($"    Exception: {log.Exception.GetType().Name}: {log.Exception.Message}");
                }
            }
            
            System.Console.WriteLine(new string('-', 80));
            System.Console.WriteLine($"Total logs in memory: {_memoryLogService.Count}");
            
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Failed to show logs: {ex.Message}");
            return 1;
        }
    }

    private static string DetectMediaType(string url)
    {
        var uri = new Uri(url);
        var extension = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
        
        return extension switch
        {
            ".mp4" or ".webm" or ".avi" or ".mkv" or ".mov" => "video/mp4",
            ".mp3" or ".aac" or ".wav" or ".flac" or ".ogg" => "audio/mpeg",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => "image/jpeg",
            _ => "video/mp4" // Default to video
        };
    }

    private static MetadataType GetMetadataType(string contentType)
    {
        return contentType switch
        {
            var ct when ct.StartsWith("video/") => MetadataType.Movie,
            var ct when ct.StartsWith("audio/") => MetadataType.Music,
            var ct when ct.StartsWith("image/") => MetadataType.Photo,
            _ => MetadataType.Default
        };
    }
}