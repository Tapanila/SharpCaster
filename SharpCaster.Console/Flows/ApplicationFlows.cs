using Microsoft.Extensions.Logging;
using Spectre.Console;
using SharpCaster.Console.Controllers;
using SharpCaster.Console.Models;
using SharpCaster.Console.Services;
using SharpCaster.Console.UI;

namespace SharpCaster.Console.Flows;

public class ApplicationFlows
{
    private readonly ApplicationState _state;
    private readonly DeviceService _deviceService;
    private readonly MediaController _mediaController;
    private readonly QueueController _queueController;
    private readonly LogViewerService _logViewerService;
    private readonly UIHelper _ui;

    public ApplicationFlows(
        ApplicationState state,
        DeviceService deviceService,
        MediaController mediaController,
        QueueController queueController,
        LogViewerService logViewerService,
        UIHelper ui)
    {
        _state = state;
        _deviceService = deviceService;
        _mediaController = mediaController;
        _queueController = queueController;
        _logViewerService = logViewerService;
        _ui = ui;
    }

    public async Task InitialDiscoveryFlowAsync()
    {
        while (_state.Devices.Count == 0)
        {
            _ui.AddSeparator("🔍 Device Discovery");
            AnsiConsole.MarkupLine("[yellow]Let's start by finding Chromecast devices on your network...[/]");
            AnsiConsole.WriteLine();

            var discoveryChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]What would you like to do?[/]")
                    .AddChoices(new[] { "Search for devices", "Exit application" })
                    .UseConverter(choice => choice switch
                    {
                        "Search for devices" => "🔍 Search for Chromecast devices",
                        "Exit application" => "❌ Exit application",
                        _ => choice
                    }));

            if (discoveryChoice == "Exit application")
            {
                await CleanupAsync();
                return;
            }
            else
            {
                await _deviceService.DiscoverDevicesAsync();
                
                if (_state.Devices.Count == 0)
                {
                    _ui.AddSeparator("❌ No Devices Found");
                    if (AnsiConsole.Confirm("[yellow]No devices found. Would you like to search again?[/]"))
                    {
                        continue;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Exiting - no devices available.[/]");
                        await CleanupAsync();
                        return;
                    }
                }
                
                _ui.AddSeparator("Searching again...");
            }
        }
    }

    public async Task DeviceSelectionFlowAsync()
    {
        if (_state.Devices.Count == 0) return;

        _ui.AddSeparator("🔗 Device Selection");
        AnsiConsole.MarkupLine($"[green]Great! Found {_state.Devices.Count} device(s). Now let's connect to one.[/]");
        AnsiConsole.WriteLine();

        // Show devices in a nice table
        _ui.DisplayDevicesTable();

        while (!_state.IsConnected)
        {
            var choices = new List<string>();
            
            // Add device options
            for (int i = 0; i < _state.Devices.Count; i++)
            {
                choices.Add($"{i + 1}. {_state.Devices[i].Name}");
            }
            
            // Add other options
            choices.AddRange(new[] { "Search for more devices", "Exit application" });

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Choose a device to connect to:[/]")
                    .PageSize(10)
                    .AddChoices(choices)
                    .UseConverter(choice => choice switch
                    {
                        var c when c.StartsWith("Search") => "🔍 Search for more devices",
                        var c when c.StartsWith("Exit") => "❌ Exit application",
                        _ => $"📺 {choice}"
                    }));

            if (choice == "Exit application")
            {
                await CleanupAsync();
                return;
            }
            
            if (choice == "Search for more devices")
            {
                _ui.AddSeparator("Searching for additional devices...");
                await _deviceService.DiscoverDevicesAsync();
                if (_state.Devices.Count > 0)
                {
                    AnsiConsole.WriteLine();
                    _ui.DisplayDevicesTable();
                }
                continue;
            }

            // Connect to selected device
            var choiceParts = choice.Split('.');
            if (choiceParts.Length > 0)
            {
                if (int.TryParse(choiceParts[0], out int deviceIndex) && deviceIndex <= _state.Devices.Count)
                {
                    _state.SelectedDevice = _state.Devices[deviceIndex - 1];
                    await _deviceService.ConnectToDeviceAsync();
                    
                    if (!_state.IsConnected)
                    {
                        AnsiConsole.WriteLine();
                        if (AnsiConsole.Confirm("[yellow]Would you like to try connecting to a different device?[/]"))
                        {
                            continue;
                        }
                        else
                        {
                            await CleanupAsync();
                            return;
                        }
                    }
                }
            }
        }
    }

    public async Task MainApplicationFlowAsync()
    {
        if (!_state.IsConnected || _state.SelectedDevice == null) return;

        _ui.AddSeparator("Ready to control your device!");
        _ui.ShowConnectedHeader();

        while (true)
        {
            await _deviceService.CheckConnectionHealthAsync();
            
            if (!_state.IsConnected)
            {
                _ui.AddSeparator("⚠️ Connection Issue Detected");
                AnsiConsole.MarkupLine("[red]⚠️  Lost connection to device. Let's reconnect...[/]");
                await Task.Delay(1500);
                _ui.AddSeparator("Attempting to reconnect...");
                await DeviceSelectionFlowAsync();
                if (_state.IsConnected)
                {
                    _ui.AddSeparator("Connected!");
                    continue;
                }
                else
                {
                    return;
                }
            }

            var choices = new[]
            {
                "Cast media",
                "Website display",
                "Media controls",
                "Stop application",
                "Queue management", 
                "Device status",
                "View logs",
                "Connect to different device",
                "Search for devices again",
                "Exit"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]What would you like to do with {_state.SelectedDevice?.Name}?[/]")
                    .PageSize(10)
                    .AddChoices(choices)
                    .UseConverter(choice => choice switch
                    {
                        "Cast media" => "📺 Cast media",
                        "Website display" => "🌐 Website display",
                        "Media controls" => "🎮 Media controls",
                        "Stop application" => "⏹️ Stop application",
                        "Queue management" => "📝 Queue management",
                        "Device status" => "📊 Device status",
                        "View logs" => "📋 View logs",
                        "Connect to different device" => "🔄 Connect to different device",
                        "Search for devices again" => "🔍 Search for devices again",
                        "Exit" => "❌ Exit",
                        _ => choice
                    }));

            _ui.AddSeparator();

            switch (choice)
            {
                case "Cast media":
                    _ui.AddSeparator("🎬 Casting Media");
                    await _mediaController.CastMediaAsync();
                    break;
                case "Website display":
                    _ui.AddSeparator("🌐 Website Display");
                    await _mediaController.CastWebsiteAsync();
                    break;
                case "Stop application":
                    _ui.AddSeparator("⏹️ Stop Application");
                    await _mediaController.StopApplicationAsync();
                    break;
                case "Media controls":
                    _ui.AddSeparator("🎮 Media Controls");
                    await _mediaController.ShowMediaControlsAsync();
                    break;
                case "Queue management":
                    _ui.AddSeparator("📝 Queue Management");
                    await _queueController.ShowQueueManagementAsync();
                    break;
                case "Device status":
                    _ui.AddSeparator("📊 Device Status");
                    await _mediaController.ShowDeviceStatusAsync();
                    break;
                case "View logs":
                    _ui.AddSeparator("📋 Application Logs");
                    await _logViewerService.ShowLogViewerAsync();
                    break;
                case "Connect to different device":
                    _state.IsConnected = false;
                    _state.Client?.Dispose();
                    _state.Client = null;
                    _state.SelectedDevice = null;
                    _ui.AddSeparator("Switching to different device...");
                    await DeviceSelectionFlowAsync();
                    if (_state.IsConnected)
                    {
                        _ui.AddSeparator("Connected to new device!");
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "Search for devices again":
                    _state.Devices.Clear();
                    _state.IsConnected = false;
                    _state.Client?.Dispose();
                    _state.Client = null;
                    _state.SelectedDevice = null;
                    _ui.AddSeparator("Searching for new devices...");
                    await InitialDiscoveryFlowAsync();
                    await DeviceSelectionFlowAsync();
                    if (_state.IsConnected)
                    {
                        _ui.AddSeparator("Connected!");
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "Exit":
                    await CleanupAsync();
                    return;
            }

            if (_state.IsConnected)
            {
                AnsiConsole.WriteLine();
                _ui.AddSeparator();
            }
        }
    }

    private Task CleanupAsync()
    {
        AnsiConsole.MarkupLine("[yellow]👋 Disconnecting and cleaning up...[/]");
        
        if (_state.Client != null)
        {
            try
            {
                _state.Client.Dispose();
            }
            catch (Exception ex)
            {
                _state.Logger?.LogError(ex, "Error during cleanup");
            }
        }
        
        _state.Locator?.Dispose();
        AnsiConsole.MarkupLine("[green]Goodbye! Thanks for using SharpCaster Console Controller.[/]");
        return Task.CompletedTask;
    }
}