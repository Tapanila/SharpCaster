using Sharpcaster;
using Sharpcaster.Models;
using Spectre.Console;
using SharpCaster.Console.Models;
using SharpCaster.Console.UI;
using System.Linq;

namespace SharpCaster.Console.Services;

public class DeviceService
{
    private readonly ApplicationState _state;
    private readonly UIHelper _ui;

    public DeviceService(ApplicationState state, UIHelper ui)
    {
        _state = state;
        _ui = ui;
    }

    public async Task DiscoverDevicesAsync()
    {
        AnsiConsole.MarkupLine("[yellow]🔍 Scanning your network for Chromecast devices...[/]");
        
        try
        {
            _state.Devices.Clear();
            
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("green"))
                .StartAsync("Scanning network (this may take a few seconds)...", async ctx =>
                {
                    var devices = await _state.Locator!.FindReceiversAsync(TimeSpan.FromSeconds(8));
                    _state.Devices.AddRange(devices);
                });

            if (_state.Devices.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]❌ No Chromecast devices found on the network.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✅ Found {_state.Devices.Count} device(s)![/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Error during discovery: {ex.Message}[/]");
        }
    }

    public async Task ConnectToDeviceAsync()
    {
        if (_state.SelectedDevice == null) return;
        
        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Star)
                .SpinnerStyle(Style.Parse("blue"))
                .StartAsync($"Connecting to {_state.SelectedDevice.Name}...", async ctx =>
                {
                    _state.Client?.Dispose();
                    _state.Client = new Sharpcaster.ChromecastClient();
                    
                    ctx.Status("Establishing connection...");
                    await _state.Client.ConnectChromecast(_state.SelectedDevice);
                    
                    ctx.Status("Verifying connection...");
                    await Task.Delay(1000); // Give connection time to stabilize
                    
                    // Verify the connection by trying to get status
                    var status = _state.Client.ReceiverChannel?.ReceiverStatus;
                    if (status == null)
                    {
                        throw new Exception("Connection established but device is not responding");
                    }
                });
            
            _state.IsConnected = true;
            _state.LastConnectionCheck = DateTime.Now;
            _ui.AddSeparator();
            AnsiConsole.MarkupLine($"[green]✅ Successfully connected to {_state.SelectedDevice.Name}![/]");
            
            // Check for existing applications and offer to join them
            await CheckAndJoinExistingApplicationAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]❌ Connection failed: {ex.Message}[/]");
            
            // Provide helpful troubleshooting tips
            AnsiConsole.MarkupLine("[dim]Troubleshooting tips:[/]");
            AnsiConsole.MarkupLine("[dim]• Make sure the device is not busy with another cast session[/]");
            AnsiConsole.MarkupLine("[dim]• Try restarting the Chromecast device[/]");
            AnsiConsole.MarkupLine("[dim]• Ensure your firewall allows the connection[/]");
            
            _state.Client?.Dispose();
            _state.Client = null;
            _state.IsConnected = false;
        }
    }

    public Task CheckConnectionHealthAsync()
    {
        if (_state.Client == null || _state.SelectedDevice == null)
        {
            _state.IsConnected = false;
            return Task.CompletedTask;
        }

        // Only check every 30 seconds to avoid spam
        if (DateTime.Now - _state.LastConnectionCheck < TimeSpan.FromSeconds(30))
        {
            return Task.CompletedTask;
        }

        _state.LastConnectionCheck = DateTime.Now;

        try
        {
            // Try to get receiver status to verify connection
            var status = _state.Client.ReceiverChannel?.ReceiverStatus;
            _state.IsConnected = status != null;
        }
        catch
        {
            _state.IsConnected = false;
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> EnsureConnectedAsync()
    {
        if (_state.Client == null || _state.SelectedDevice == null || !_state.IsConnected)
        {
            AnsiConsole.MarkupLine("[red]❌ Not connected to any device. Please connect first.[/]");
            return Task.FromResult(false);
        }

        // Quick connection test
        try
        {
            var status = _state.Client.ReceiverChannel?.ReceiverStatus;
            if (status == null)
            {
                _state.IsConnected = false;
                AnsiConsole.MarkupLine("[red]❌ Connection lost. Please reconnect to the device.[/]");
                return Task.FromResult(false);
            }
        }
        catch
        {
            _state.IsConnected = false;
            AnsiConsole.MarkupLine("[red]❌ Connection appears to be lost. Please reconnect to the device.[/]");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private async Task CheckAndJoinExistingApplicationAsync()
    {
        if (_state.Client == null || !_state.IsConnected)
            return;

        try
        {
            // Refresh receiver status to get the most current application information
            await _state.Client.ReceiverChannel.GetChromecastStatusAsync();
            var receiverStatus = _state.Client.ReceiverChannel.ReceiverStatus;
            
            if (receiverStatus?.Applications != null && receiverStatus.Applications.Count > 0)
            {
                var runningApp = receiverStatus.Applications.FirstOrDefault();
                if (runningApp != null && !string.IsNullOrEmpty(runningApp.DisplayName))
                {
                    _ui.AddSeparator("🎯 Existing Application Detected");
                    AnsiConsole.MarkupLine($"[yellow]Found existing application: [bold]{runningApp.DisplayName}[/][/]");
                    AnsiConsole.MarkupLine($"[dim]App ID: {runningApp.AppId}[/]");
                    if (!string.IsNullOrEmpty(runningApp.StatusText))
                    {
                        AnsiConsole.MarkupLine($"[dim]Status: {runningApp.StatusText}[/]");
                    }
                    AnsiConsole.WriteLine();

                    var shouldJoin = AnsiConsole.Confirm("[yellow]Would you like to join this existing application?[/]");
                    
                    if (shouldJoin)
                    {
                        await AnsiConsole.Status()
                            .Spinner(Spinner.Known.Star)
                            .SpinnerStyle(Style.Parse("green"))
                            .StartAsync("Joining existing application...", async ctx =>
                            {
                                // Use LaunchApplicationAsync with joinExistingApplicationSession = true
                                // This will join the existing app instead of launching a new one
                                await _state.Client.LaunchApplicationAsync(runningApp.AppId, joinExistingApplicationSession: true);
                            });
                        
                        AnsiConsole.MarkupLine($"[green]✅ Successfully joined {runningApp.DisplayName}![/]");
                        
                        // If it's a media application, try to get current media status
                        var hasMediaNamespace = runningApp.Namespaces?.Any(ns => ns.Name == "urn:x-cast:com.google.cast.media") == true;
                        if (hasMediaNamespace)
                        {
                            try
                            {
                                var mediaStatus = await _state.Client.MediaChannel.GetMediaStatusAsync();
                                if (mediaStatus != null)
                                {
                                    AnsiConsole.MarkupLine($"[cyan]📺 Current media: {mediaStatus.Media?.Metadata?.Title ?? "Unknown"}[/]");
                                    AnsiConsole.MarkupLine($"[cyan]🎮 Player state: {mediaStatus.PlayerState}[/]");
                                }
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[dim]Could not retrieve media status: {ex.Message}[/]");
                            }
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[dim]Skipping existing application. You can cast new media or control the device normally.[/]");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[dim]Could not check for existing applications: {ex.Message}[/]");
        }
    }
}