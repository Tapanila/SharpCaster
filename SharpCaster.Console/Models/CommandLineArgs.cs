namespace SharpCaster.Console.Models;

public class CommandLineArgs
{
    public string? DeviceName { get; set; }
    public string? DeviceIpAddress { get; set; }
    public string? Command { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaTitle { get; set; }
    public double? Volume { get; set; }
    public double? SeekTime { get; set; }
    public bool ShowHelp { get; set; }
    public bool ShowDevices { get; set; }
    public bool ShowVersion { get; set; }
    public bool ShowLogs { get; set; }
    public bool IsInteractive => string.IsNullOrEmpty(Command);
}

public static class CommandLineParser
{
    public static CommandLineArgs Parse(string[] args)
    {
        var result = new CommandLineArgs();
        
        if (args.Length == 0)
        {
            return result; // Interactive mode
        }

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i].ToLowerInvariant();
            
            switch (arg)
            {
                case "-h":
                case "--help":
                case "help":
                    result.ShowHelp = true;
                    return result;
                    
                case "--version":
                case "-v":
                case "version":
                    result.ShowVersion = true;
                    return result;
                    
                case "--list-devices":
                case "list":
                    result.ShowDevices = true;
                    break;
                    
                case "play":
                case "pause":
                case "stop":
                case "stop-app":
                case "status":
                case "website":
                    result.Command = arg;
                    break;
                    
                case "cast":
                    result.Command = "play";
                    break;
                    
                case "volume":
                case "vol":
                    result.Command = "volume";
                    break;
                    
                case "seek":
                    result.Command = "seek";
                    break;
                    
                case "--ip":
                case "-i":
                    if (i + 1 < args.Length)
                    {
                        result.DeviceIpAddress = args[++i];
                    }
                    break;
                    
                case "--title":
                case "-t":
                    if (i + 1 < args.Length)
                    {
                        result.MediaTitle = args[++i];
                    }
                    break;
                    
                case "--logs":
                case "-l":
                    result.ShowLogs = true;
                    break;
                    
                default:
                    // Check if this is a numeric argument for volume or seek
                    if (double.TryParse(args[i], out double numericValue))
                    {
                        if (result.Command == "volume" && !result.Volume.HasValue)
                        {
                            result.Volume = numericValue;
                        }
                        else if (result.Command == "seek" && !result.SeekTime.HasValue)
                        {
                            result.SeekTime = numericValue;
                        }
                    }
                    // First non-command argument is device name
                    else if (string.IsNullOrEmpty(result.DeviceName) && !arg.StartsWith("-") && !IsUrl(arg))
                    {
                        result.DeviceName = args[i]; // Use original case
                    }
                    // URLs are media URLs
                    else if (IsUrl(arg))
                    {
                        result.MediaUrl = args[i]; // Use original case
                        if (string.IsNullOrEmpty(result.Command))
                        {
                            result.Command = "play"; // Default to play when URL is provided
                        }
                    }
                    break;
            }
        }
        
        return result;
    }
    
    private static bool IsUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && 
               (uri.Scheme == "http" || uri.Scheme == "https");
    }
    
    private static bool IsIPAddress(string value)
    {
        return System.Net.IPAddress.TryParse(value, out _);
    }
    
    public static void ShowHelp()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("sharpcaster - Command Line Usage");
        System.Console.WriteLine();
        System.Console.WriteLine("Interactive Mode:");
        System.Console.WriteLine("  sharpcaster");
        System.Console.WriteLine();
        System.Console.WriteLine("Command Line Mode:");
        System.Console.WriteLine("  sharpcaster <device-name> <command> [options]");
        System.Console.WriteLine();
        System.Console.WriteLine("Commands:");
        System.Console.WriteLine("  play <url>              Cast and play media from URL");
        System.Console.WriteLine("  cast <url>              Alias for 'play'");
        System.Console.WriteLine("  pause                   Pause current media");
        System.Console.WriteLine("  stop                    Stop current media");
        System.Console.WriteLine("  stop-app                Stop the currently running application");
        System.Console.WriteLine("  volume <0.0-1.0>        Set device volume (0.0 = mute, 1.0 = max)");
        System.Console.WriteLine("  media-volume <0.0-1.0>  Set media stream volume (0.0 = mute, 1.0 = max)");
        System.Console.WriteLine("  seek <seconds>          Seek to specific time in seconds");
        System.Console.WriteLine("  status                  Show current media status");
        System.Console.WriteLine("  website <url>           Open and display a website");
        System.Console.WriteLine("  list                    List available devices");
        System.Console.WriteLine("  version                 Show application version");
        System.Console.WriteLine("  help                    Show this help");
        System.Console.WriteLine();
        System.Console.WriteLine("Options:");
        System.Console.WriteLine("  --ip <ip-address>       Connect directly to device IP (skips discovery)");
        System.Console.WriteLine("  -i <ip-address>         Short form of --ip");
        System.Console.WriteLine("  --title <title>         Set media title (for play/cast commands)");
        System.Console.WriteLine("  -t <title>              Short form of --title");
        System.Console.WriteLine("  --logs                  Show application logs after command execution");
        System.Console.WriteLine("  -l                      Short form of --logs");
        System.Console.WriteLine();
        System.Console.WriteLine("Examples:");
        System.Console.WriteLine("  # Interactive mode");
        System.Console.WriteLine("  sharpcaster");
        System.Console.WriteLine();
        System.Console.WriteLine("  # List available devices");
        System.Console.WriteLine("  sharpcaster list");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Cast media to specific device");
        System.Console.WriteLine("  sharpcaster \"Living Room TV\" play \"https://example.com/video.mp4\"");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Cast with custom title");
        System.Console.WriteLine("  sharpcaster \"Office TV\" play \"https://example.com/video.mp4\" --title \"My Video\"");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Display a website");
        System.Console.WriteLine("  sharpcaster \"Office TV\" website \"https://www.google.com\"");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Connect directly using IP address (skips discovery)");
        System.Console.WriteLine("  sharpcaster --ip 192.168.1.100 play \"https://example.com/video.mp4\"");
        System.Console.WriteLine("  sharpcaster -i 192.168.1.100 status");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Control playback");
        System.Console.WriteLine("  sharpcaster \"Kitchen Speaker\" pause");
        System.Console.WriteLine("  sharpcaster \"Bedroom TV\" volume 0.7");
        System.Console.WriteLine("  sharpcaster \"Living Room TV\" media-volume 0.5");
        System.Console.WriteLine("  sharpcaster \"Living Room TV\" seek 120");
        System.Console.WriteLine("  sharpcaster \"Office TV\" stop-app");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Check status and view logs");
        System.Console.WriteLine("  sharpcaster \"Office TV\" status");
        System.Console.WriteLine("  sharpcaster \"Office TV\" status --logs");
        System.Console.WriteLine("  sharpcaster --ip 192.168.1.100 play \"video.mp4\" --logs");
        System.Console.WriteLine();
        System.Console.WriteLine("Device Matching:");
        System.Console.WriteLine("  - Device names are matched case-insensitively");
        System.Console.WriteLine("  - Partial matches are supported (e.g., \"office\" matches \"Office TV\")");
        System.Console.WriteLine("  - Use quotes for device names with spaces");
        System.Console.WriteLine();
        System.Console.WriteLine("Supported Media:");
        System.Console.WriteLine("  - Video: MP4, WebM, AVI, MKV and other formats supported by Chromecast");
        System.Console.WriteLine("  - Audio: MP3, AAC, WAV, FLAC, OGG and other audio formats");
        System.Console.WriteLine("  - Images: JPG, PNG, GIF, WebP and other image formats");
        System.Console.WriteLine();
    }

    public static void ShowVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        
        // For single-file apps, use AppContext.BaseDirectory instead of Assembly.Location
        var buildDate = DateTime.Now; // Default fallback
        try
        {
#pragma warning disable IL3000 // Assembly.Location is accessed - handled with fallback for single-file apps
            var location = assembly.Location;
#pragma warning restore IL3000
            if (!string.IsNullOrEmpty(location))
            {
                buildDate = System.IO.File.GetLastWriteTime(location);
            }
            else
            {
                // Single-file app - try to get build date from base directory
                var baseDir = System.AppContext.BaseDirectory;
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath) && System.IO.File.Exists(exePath))
                {
                    buildDate = System.IO.File.GetLastWriteTime(exePath);
                }
            }
        }
        catch
        {
            // Fallback to current time if we can't determine build date
        }
        
        System.Console.WriteLine($"sharpcaster v{version}");
        System.Console.WriteLine($"Build Date: {buildDate:yyyy-MM-dd HH:mm:ss}");
        System.Console.WriteLine($"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        System.Console.WriteLine($"Platform: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
        System.Console.WriteLine($"Architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
        System.Console.WriteLine();
        System.Console.WriteLine("A .NET Chromecast controller library and console application");
        System.Console.WriteLine("https://github.com/Tapanila/SharpCaster");
    }
}