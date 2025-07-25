namespace SharpCaster.Console.Models;

public class CommandLineArgs
{
    public string? DeviceName { get; set; }
    public string? Command { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaTitle { get; set; }
    public double? Volume { get; set; }
    public double? SeekTime { get; set; }
    public bool ShowHelp { get; set; }
    public bool ShowDevices { get; set; }
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
                    
                case "--list-devices":
                case "list":
                    result.ShowDevices = true;
                    return result;
                    
                case "play":
                case "pause":
                case "stop":
                case "status":
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
                    
                case "--title":
                case "-t":
                    if (i + 1 < args.Length)
                    {
                        result.MediaTitle = args[++i];
                    }
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
    
    public static void ShowHelp()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("SharpCaster Console Controller - Command Line Usage");
        System.Console.WriteLine();
        System.Console.WriteLine("Interactive Mode:");
        System.Console.WriteLine("  SharpCaster.Console");
        System.Console.WriteLine();
        System.Console.WriteLine("Command Line Mode:");
        System.Console.WriteLine("  SharpCaster.Console <device-name> <command> [options]");
        System.Console.WriteLine();
        System.Console.WriteLine("Commands:");
        System.Console.WriteLine("  play <url>              Cast and play media from URL");
        System.Console.WriteLine("  cast <url>              Alias for 'play'");
        System.Console.WriteLine("  pause                   Pause current media");
        System.Console.WriteLine("  stop                    Stop current media");
        System.Console.WriteLine("  volume <0.0-1.0>        Set volume (0.0 = mute, 1.0 = max)");
        System.Console.WriteLine("  seek <seconds>          Seek to specific time in seconds");
        System.Console.WriteLine("  status                  Show current media status");
        System.Console.WriteLine("  list                    List available devices");
        System.Console.WriteLine("  help                    Show this help");
        System.Console.WriteLine();
        System.Console.WriteLine("Options:");
        System.Console.WriteLine("  --title <title>         Set media title (for play/cast commands)");
        System.Console.WriteLine("  -t <title>              Short form of --title");
        System.Console.WriteLine();
        System.Console.WriteLine("Examples:");
        System.Console.WriteLine("  # Interactive mode");
        System.Console.WriteLine("  SharpCaster.Console");
        System.Console.WriteLine();
        System.Console.WriteLine("  # List available devices");
        System.Console.WriteLine("  SharpCaster.Console list");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Cast media to specific device");
        System.Console.WriteLine("  SharpCaster.Console \"Living Room TV\" play \"https://example.com/video.mp4\"");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Cast with custom title");
        System.Console.WriteLine("  SharpCaster.Console \"Office TV\" play \"https://example.com/video.mp4\" --title \"My Video\"");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Control playback");
        System.Console.WriteLine("  SharpCaster.Console \"Kitchen Speaker\" pause");
        System.Console.WriteLine("  SharpCaster.Console \"Bedroom TV\" volume 0.7");
        System.Console.WriteLine("  SharpCaster.Console \"Living Room TV\" seek 120");
        System.Console.WriteLine();
        System.Console.WriteLine("  # Check status");
        System.Console.WriteLine("  SharpCaster.Console \"Office TV\" status");
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
}