using Sharpcaster.Extensions;
using Sharpcaster.Messages.Web;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using Spectre.Console;
using SharpCaster.Console.Models;
using SharpCaster.Console.Services;
using SharpCaster.Console.UI;
using System.Text.Json;

namespace SharpCaster.Console.Controllers;

public class MediaController
{
    private readonly ApplicationState _state;
    private readonly DeviceService _deviceService;
    private readonly UIHelper _ui;

    public MediaController(ApplicationState state, DeviceService deviceService, UIHelper ui)
    {
        _state = state;
        _deviceService = deviceService;
        _ui = ui;
    }

    public async Task CastMediaAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        var urlOptions = new[]
        {
            "Sample Video (Designing for Google Cast)",
            "Sample Audio (Arcane - Kevin MacLeod)",
            "Custom URL"
        };

        var urlChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select media to cast:[/]")
                .AddChoices(urlOptions)
                .UseConverter(choice => choice switch
                {
                    "Sample Video (Designing for Google Cast)" => "🎬 Sample Video (Designing for Google Cast)",
                    "Sample Audio (Arcane - Kevin MacLeod)" => "🎵 Sample Audio (Arcane - Kevin MacLeod)",
                    "Custom URL" => "🔗 Custom URL",
                    _ => choice
                }));

        string url;
        string title;

        switch (urlChoice)
        {
            case "Sample Video (Designing for Google Cast)":
                url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4";
                title = "Designing for Google Cast";
                break;
            case "Sample Audio (Arcane - Kevin MacLeod)":
                url = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Arcane.mp3";
                title = "Arcane";
                break;
            case "Custom URL":
                url = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Enter media URL:[/]")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]Please enter a valid URL[/]")
                        .Validate(url => Uri.TryCreate(url, UriKind.Absolute, out _)));
                title = "Custom Media";
                break;
            default:
                throw new InvalidOperationException("Invalid URL choice");
        }

        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Star2)
                .SpinnerStyle(Style.Parse("yellow"))
                .StartAsync("Loading content...", async ctx =>
                {
                    var contentType = DetectMediaType(url);
                    var media = new Media
                    {
                        ContentId = url,
                        ContentType = contentType,
                        StreamType = StreamType.Buffered,
                        Metadata = new MediaMetadata
                        {
                            MetadataType = GetMetadataType(contentType),
                            Title = title
                        }
                    };

                    ctx.Status("Loading media...");
                    var status = await _state.Client.MediaChannel.LoadAsync(media);
                    
                    if (status == null)
                        throw new Exception("Failed to load media - no status returned");
                });
            
            _ui.AddSeparator();
            AnsiConsole.MarkupLine("[green]✅ Media loaded and playing successfully![/]");
        }
        catch (Exception ex)
        {
            _ui.AddSeparator("❌ Casting Error");
            AnsiConsole.MarkupLine($"[red]❌ Casting failed: {ex.Message}[/]");
            
            if (ex.Message.Contains("timeout") || ex.Message.Contains("connection"))
            {
                _state.IsConnected = false;
                AnsiConsole.MarkupLine("[yellow]⚠️  Connection may have been lost. Try reconnecting.[/]");
            }
        }
    }

    public async Task CastWebsiteAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        var url = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Enter website URL:[/]")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Please enter a valid URL[/]")
                .Validate(url => 
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                        (uri.Scheme != "http" && uri.Scheme != "https"))
                    {
                        return ValidationResult.Error("[red]Must be a valid http or https URL[/]");
                    }
                    return ValidationResult.Success();
                }));

        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Star2)
                .SpinnerStyle(Style.Parse("yellow"))
                .StartAsync("Loading website on Chromecast...", async ctx =>
                {
                    const string dashboardReceiver = "F7FD2183";

                    if (_state.Client!.ChromecastStatus.Application?.AppId == dashboardReceiver)
                    {
                        ctx.Status("Dashboard receiver already running");
                    }
                    else
                    {
                        if (_state.Client.ChromecastStatus.Application != null && _state.Client.ChromecastStatus.Application.AppId != "E8C28D3C")
                        {
                            ctx.Status("Stopping previous application");
                            await _state.Client.ReceiverChannel.StopApplication();
                            await Task.Delay(300); // Wait for stop to complete
                            await _state.Client.DisconnectAsync();
                            await Task.Delay(500); // Wait disconnect to complete
                            await _state.Client.ConnectChromecast(_state.SelectedDevice);
                        }

                        ctx.Status("Launching dashboard receiver...");
                        var status = await _state.Client.LaunchApplicationAsync(dashboardReceiver);
                        if (status == null)
                            throw new Exception("Failed to launch dashboard receiver");
                    }

                    var req = new WebMessage
                    {
                        Url = url,
                        Type = "load",
                        SessionId = _state.Client.ChromecastStatus.Application.SessionId
                    };

                    var requestPayload = JsonSerializer.Serialize(req, SharpcasteSerializationContext.Default.WebMessage);

                    ctx.Status("Loading website...");
                    await _state.Client.SendAsync(null, "urn:x-cast:com.boombatower.chromecast-dashboard", requestPayload, _state.Client.ChromecastStatus.Application.SessionId);
                });
            
            _ui.AddSeparator();
            AnsiConsole.MarkupLine("[green]✅ Website loaded successfully![/]");
            AnsiConsole.MarkupLine($"[dim]Displaying: {url}[/]");
        }
        catch (Exception ex)
        {
            _ui.AddSeparator("❌ Website Loading Error");
            AnsiConsole.MarkupLine($"[red]❌ Website loading failed: {ex.Message}[/]");
            
            if (ex.Message.Contains("timeout") || ex.Message.Contains("connection"))
            {
                _state.IsConnected = false;
                AnsiConsole.MarkupLine("[yellow]⚠️  Connection may have been lost. Try reconnecting.[/]");
            }
        }
    }

    public async Task StopApplicationAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Star2)
                .SpinnerStyle(Style.Parse("yellow"))
                .StartAsync("Stopping application...", async ctx =>
                {
                    var receiverStatus = _state.Client!.ReceiverChannel.ReceiverStatus;
                    if (receiverStatus?.Applications?.Any() == true)
                    {
                        var app = receiverStatus.Applications.First();
                        ctx.Status($"Stopping '{app.DisplayName}'...");
                        await _state.Client.ReceiverChannel.StopApplication();
                    }
                    else
                    {
                        ctx.Status("No applications running");
                    }
                });
            
            _ui.AddSeparator();
            var status = _state.Client!.ReceiverChannel.ReceiverStatus;
            if (status?.Applications?.Any() == true)
            {
                var app = status.Applications.First();
                AnsiConsole.MarkupLine($"[green]✅ Application '{app.DisplayName}' stopped successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]ℹ️ No applications were running[/]");
            }
        }
        catch (Exception ex)
        {
            _ui.AddSeparator("❌ Stop Application Error");
            AnsiConsole.MarkupLine($"[red]❌ Failed to stop application: {ex.Message}[/]");
            
            if (ex.Message.Contains("timeout") || ex.Message.Contains("connection"))
            {
                _state.IsConnected = false;
                AnsiConsole.MarkupLine("[yellow]⚠️  Connection may have been lost. Try reconnecting.[/]");
            }
        }
    }

    public async Task ShowMediaControlsAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        while (true)
        {
            var choices = new[]
            {
                "Play",
                "Pause", 
                "Stop",
                "Seek",
                "Set device volume",
                "Set media volume",
                "Mute/Unmute device",
                "Mute/Unmute media",
                "Get media status",
                "Back to main menu"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Media Controls:[/]")
                    .AddChoices(choices)
                    .UseConverter(choice => choice switch
                    {
                        "Play" => "▶️ Play",
                        "Pause" => "⏸️ Pause",
                        "Stop" => "⏹️ Stop",
                        "Seek" => "⏭️ Seek",
                        "Set device volume" => "🔊 Set device volume",
                        "Set media volume" => "🎵 Set media volume",
                        "Mute/Unmute device" => "🔇 Mute/Unmute device",
                        "Mute/Unmute media" => "🔈 Mute/Unmute media",
                        "Get media status" => "📊 Get media status",
                        "Back to main menu" => "🔙 Back to main menu",
                        _ => choice
                    }));

            try
            {
                var mediaChannel = _state.Client!.MediaChannel;
                
                switch (choice)
                {
                    case "Play":
                        await AnsiConsole.Status().StartAsync("Playing...", async ctx =>
                        {
                            var status = await mediaChannel.PlayAsync();
                            if (status == null) throw new Exception("Failed to play");
                        });
                        AnsiConsole.MarkupLine("[green]▶️ Playing[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Pause":
                        await AnsiConsole.Status().StartAsync("Pausing...", async ctx =>
                        {
                            var status = await mediaChannel.PauseAsync();
                            if (status == null) throw new Exception("Failed to pause");
                        });
                        AnsiConsole.MarkupLine("[yellow]⏸️ Paused[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Stop":
                        await AnsiConsole.Status().StartAsync("Stopping...", async ctx =>
                        {
                            await mediaChannel.StopAsync();
                        });
                        AnsiConsole.MarkupLine("[red]⏹️ Stopped[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Seek":
                        var seekTime = AnsiConsole.Prompt(
                            new TextPrompt<double>("[yellow]Enter seek time in seconds:[/]")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Please enter a valid number[/]"));
                        
                        await AnsiConsole.Status().StartAsync($"Seeking to {seekTime}s...", async ctx =>
                        {
                            await mediaChannel.SeekAsync(seekTime);
                        });
                        AnsiConsole.MarkupLine($"[green]⏭️ Seeked to {seekTime:F1}s[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Set device volume":
                        var deviceVolume = AnsiConsole.Prompt(
                            new TextPrompt<double>("[yellow]Enter device volume (0.0 - 1.0):[/]")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Volume must be between 0.0 and 1.0[/]")
                                .Validate(v => v >= 0 && v <= 1));
                        
                        await AnsiConsole.Status().StartAsync($"Setting device volume to {deviceVolume:P0}...", async ctx =>
                        {
                            await _state.Client.ReceiverChannel.SetVolume(deviceVolume);
                        });
                        AnsiConsole.MarkupLine($"[green]🔊 Device volume set to {deviceVolume:P0}[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Set media volume":
                        var mediaVolume = AnsiConsole.Prompt(
                            new TextPrompt<double>("[yellow]Enter media stream volume (0.0 - 1.0):[/]")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Volume must be between 0.0 and 1.0[/]")
                                .Validate(v => v >= 0 && v <= 1));
                        
                        await AnsiConsole.Status().StartAsync($"Setting media volume to {mediaVolume:P0}...", async ctx =>
                        {
                            var status = await mediaChannel.SetVolumeAsync(mediaVolume);
                            if (status == null) throw new Exception("Failed to set media volume");
                        });
                        AnsiConsole.MarkupLine($"[green]🎵 Media volume set to {mediaVolume:P0}[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Mute/Unmute device":
                        var currentDeviceStatus = _state.Client.ReceiverChannel.ReceiverStatus;
                        var isDeviceMuted = currentDeviceStatus?.Volume?.Muted == true;
                        var newDeviceMuteState = !isDeviceMuted;
                        
                        await AnsiConsole.Status().StartAsync($"{(newDeviceMuteState ? "Muting" : "Unmuting")} device...", async ctx =>
                        {
                            await _state.Client.ReceiverChannel.SetMute(newDeviceMuteState);
                        });
                        AnsiConsole.MarkupLine($"[green]🔇 Device {(newDeviceMuteState ? "muted" : "unmuted")}[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Mute/Unmute media":
                        var currentMediaStatus = await mediaChannel.GetMediaStatusAsync();
                        var isMediaMuted = currentMediaStatus?.Volume?.Muted == true;
                        var newMediaMuteState = !isMediaMuted;
                        
                        await AnsiConsole.Status().StartAsync($"{(newMediaMuteState ? "Muting" : "Unmuting")} media stream...", async ctx =>
                        {
                            var status = await mediaChannel.SetMuteAsync(newMediaMuteState);
                            if (status == null) throw new Exception("Failed to set media mute state");
                        });
                        AnsiConsole.MarkupLine($"[green]🔈 Media stream {(newMediaMuteState ? "muted" : "unmuted")}[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Get media status":
                        var status = await mediaChannel.GetMediaStatusAsync();
                        if (status != null)
                        {
                            var statusTable = new Table();
                            statusTable.AddColumn("[blue]Property[/]");
                            statusTable.AddColumn("[blue]Value[/]");
                            
                            statusTable.AddRow("[cyan]State[/]", $"[white]{status.PlayerState}[/]");
                            statusTable.AddRow("[cyan]Current Time[/]", $"[white]{status.CurrentTime:F1}s[/]");
                            statusTable.AddRow("[cyan]Duration[/]", $"[white]{status.Media?.Duration:F1}s[/]");
                            statusTable.AddRow("[cyan]Title[/]", $"[white]{status.Media?.Metadata?.Title ?? "Unknown"}[/]");
                            
                            if (status.Media?.Duration > 0)
                            {
                                var progress = (status.CurrentTime / status.Media.Duration.Value) * 100;
                                statusTable.AddRow("[cyan]Progress[/]", $"[white]{progress:F1}%[/]");
                            }
                            
                            AnsiConsole.Write(statusTable);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]❌ No media status available.[/]");
                        }
                        break;
                        
                    case "Back to main menu":
                        return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Operation failed: {ex.Message}[/]");
                
                if (ex.Message.Contains("timeout") || ex.Message.Contains("connection"))
                {
                    _state.IsConnected = false;
                    AnsiConsole.MarkupLine("[yellow]⚠️  Connection may have been lost. Returning to main menu.[/]");
                    return;
                }
            }

            if (choice != "Get media status")
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                System.Console.ReadKey();
            }
        }
    }

    public async Task ShowDeviceStatusAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        try
        {
            var status = _state.Client!.ReceiverChannel.ReceiverStatus;
            
            var panel = new Panel(_ui.CreateDeviceStatusTable(status))
                .Header($"[blue]📱 Device Status - {_state.SelectedDevice!.Name}[/]")
                .BorderColor(Color.Blue)
                .Padding(1, 1);
            
            AnsiConsole.Write(panel);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Failed to get device status: {ex.Message}[/]");
            
            if (ex.Message.Contains("timeout") || ex.Message.Contains("connection"))
            {
                _state.IsConnected = false;
                AnsiConsole.MarkupLine("[yellow]⚠️  Connection may have been lost.[/]");
            }
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