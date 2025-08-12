using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using Spectre.Console;
using SharpCaster.Console.Models;
using SharpCaster.Console.Services;
using SharpCaster.Console.UI;

namespace SharpCaster.Console.Controllers;

public class QueueController
{
    private readonly ApplicationState _state;
    private readonly DeviceService _deviceService;
    private readonly UIHelper _ui;

    public QueueController(ApplicationState state, DeviceService deviceService, UIHelper ui)
    {
        _state = state;
        _deviceService = deviceService;
        _ui = ui;
    }

    public async Task ShowQueueManagementAsync()
    {
        if (!await _deviceService.EnsureConnectedAsync())
            return;

        while (true)
        {
            var choices = new[]
            {
                "Load queue from URLs",
                "Next track",
                "Previous track", 
                "Toggle shuffle",
                "Set repeat mode",
                "Get queue items",
                "Back to main menu"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Queue Management:[/]")
                    .AddChoices(choices)
                    .UseConverter(choice => choice switch
                    {
                        "Load queue from URLs" => "📝 Load queue from URLs",
                        "Next track" => "⏭️ Next track",
                        "Previous track" => "⏮️ Previous track",
                        "Toggle shuffle" => "🔀 Toggle shuffle",
                        "Set repeat mode" => "🔁 Set repeat mode",
                        "Get queue items" => "📋 Get queue items",
                        "Back to main menu" => "🔙 Back to main menu",
                        _ => choice
                    }));

            try
            {
                var mediaChannel = _state.Client!.MediaChannel;
                
                switch (choice)
                {
                    case "Load queue from URLs":
                        await LoadQueueAsync(mediaChannel);
                        break;
                        
                    case "Next track":
                        await AnsiConsole.Status().StartAsync("Skipping to next track...", async ctx =>
                        {
                            await mediaChannel.QueueNextAsync();
                        });
                        AnsiConsole.MarkupLine("[green]⏭️ Skipped to next track[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Previous track":
                        await AnsiConsole.Status().StartAsync("Going to previous track...", async ctx =>
                        {
                            await mediaChannel.QueuePrevAsync();
                        });
                        AnsiConsole.MarkupLine("[green]⏮️ Went to previous track[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Toggle shuffle":
                        var shuffle = AnsiConsole.Confirm("[yellow]Enable shuffle?[/]");
                        await AnsiConsole.Status().StartAsync($"{(shuffle ? "Enabling" : "Disabling")} shuffle...", async ctx =>
                        {
                            await mediaChannel.QueueShuffleAsync(shuffle);
                        });
                        AnsiConsole.MarkupLine($"[green]🔀 Shuffle {(shuffle ? "enabled" : "disabled")}[/]");
                        _ui.AddSeparator();
                        break;
                        
                    case "Set repeat mode":
                        await SetRepeatModeAsync(mediaChannel);
                        break;
                        
                    case "Get queue items":
                        var itemIds = await mediaChannel.QueueGetItemIdsAsync();
                        if (itemIds?.Any() == true)
                        {
                            var queueTable = new Table();
                            queueTable.AddColumn("[blue]Item ID[/]");
                            
                            foreach (var id in itemIds)
                            {
                                queueTable.AddRow($"[white]{id}[/]");
                            }
                            
                            AnsiConsole.MarkupLine($"[green]📋 Queue contains {itemIds.Length} items:[/]");
                            AnsiConsole.Write(queueTable);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[yellow]📋 Queue is empty or unavailable.[/]");
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

            if (choice != "Get queue items")
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                System.Console.ReadKey();
            }
        }
    }

    private async Task LoadQueueAsync(Sharpcaster.Channels.MediaChannel mediaChannel)
    {
        var trackCount = AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]How many tracks do you want to add?[/]")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Please enter a valid number greater than 0[/]")
                .Validate(n => n > 0));

        var queueItems = new List<QueueItem>();
        
        for (int i = 0; i < trackCount; i++)
        {
            AnsiConsole.MarkupLine($"[cyan]Track {i + 1} of {trackCount}:[/]");
            
            var url = AnsiConsole.Prompt(
                new TextPrompt<string>($"[yellow]Enter URL for track {i + 1}:[/]")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Please enter a valid URL[/]")
                    .Validate(url => Uri.TryCreate(url, UriKind.Absolute, out _)));
            
            var title = AnsiConsole.Prompt(
                new TextPrompt<string>($"[yellow]Enter title for track {i + 1} (optional):[/]")
                    .PromptStyle("green")
                    .AllowEmpty());
            
            queueItems.Add(new QueueItem
            {
                Media = new Media
                {
                    ContentId = url,
                    ContentType = "audio/mpeg",
                    StreamType = StreamType.Buffered,
                    Metadata = new MediaMetadata
                    {
                        MetadataType = MetadataType.Music,
                        Title = string.IsNullOrWhiteSpace(title) ? $"Track {i + 1}" : title
                    }
                }
            });
        }

        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Balloon)
                .SpinnerStyle(Style.Parse("green"))
                .StartAsync("Loading queue...", async ctx =>
                {
                    ctx.Status($"Loading queue with {queueItems.Count} items...");
                    await mediaChannel.QueueLoadAsync(queueItems.ToArray(), RepeatModeType.OFF, 0);
                });
            
            _ui.AddSeparator();
            AnsiConsole.MarkupLine($"[green]✅ Queue loaded with {queueItems.Count} items[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Failed to load queue: {ex.Message}[/]");
        }
    }

    private async Task SetRepeatModeAsync(Sharpcaster.Channels.MediaChannel mediaChannel)
    {
        var repeatModes = new[] { RepeatModeType.OFF, RepeatModeType.ALL, RepeatModeType.SINGLE };

        var repeatMode = AnsiConsole.Prompt(
            new SelectionPrompt<RepeatModeType>()
                .Title("[yellow]Select repeat mode:[/]")
                .AddChoices(repeatModes)
                .UseConverter(mode => mode switch
                {
                    RepeatModeType.OFF => "🚫 Off",
                    RepeatModeType.ALL => "🔁 Repeat All",
                    RepeatModeType.SINGLE => "🔂 Repeat Single",
                    _ => mode.ToString()
                }));

        try
        {
            await AnsiConsole.Status().StartAsync($"Setting repeat mode to {repeatMode}...", async ctx =>
            {
                await mediaChannel.QueueSetRepeatModeAsync(repeatMode);
            });
            
            var modeText = repeatMode switch
            {
                RepeatModeType.OFF => "Off",
                RepeatModeType.ALL => "All",
                RepeatModeType.SINGLE => "Single",
                _ => repeatMode.ToString()
            };
            
            AnsiConsole.MarkupLine($"[green]🔁 Repeat mode set to {modeText}[/]");
            _ui.AddSeparator();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Failed to set repeat mode: {ex.Message}[/]");
        }
    }
}