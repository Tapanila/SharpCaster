using Sharpcaster.Models;
using Sharpcaster.Models.ChromecastStatus;
using Spectre.Console;
using SharpCaster.Console.Models;

namespace SharpCaster.Console.UI;

public class UIHelper
{
    private readonly ApplicationState _state;

    public UIHelper(ApplicationState state)
    {
        _state = state;
    }

    public void AddSeparator(string? title = null)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            var rule = new Rule($"[dim]{title}[/]")
            {
                Style = Style.Parse("grey"),
                Justification = Justify.Left
            };
            AnsiConsole.Write(rule);
        }
        else
        {
            AnsiConsole.Write(new Rule() { Style = Style.Parse("grey") });
        }
        AnsiConsole.WriteLine();
    }

    public void ShowWelcome()
    {
        var rule = new Rule("[bold blue]SharpCaster Console Controller[/]")
        {
            Style = Style.Parse("blue"),
            Justification = Justify.Center
        };
        AnsiConsole.Write(rule);
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]A powerful console interface for controlling Chromecast devices[/]");
        
        // Show version information
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        AnsiConsole.MarkupLine($"[dim]Version: [/][yellow]{version}[/]");
        AnsiConsole.WriteLine();
        
        // Show the flow steps
        var flowPanel = new Panel(
            "[yellow]Getting Started:[/]\n" +
            "[dim]1.[/] [cyan]Discover devices[/] on your network\n" +
            "[dim]2.[/] [cyan]Select a device[/] to connect to\n" +
            "[dim]3.[/] [cyan]Control your Chromecast[/] with full features")
            .Header("[blue]🚀 Quick Start Guide[/]")
            .BorderColor(Color.Blue)
            .Padding(1, 1);
        
        AnsiConsole.Write(flowPanel);
        AnsiConsole.WriteLine();
    }

    public void ShowConnectedHeader()
    {
        var rule = new Rule($"[bold green]Connected to: {_state.SelectedDevice?.Name}[/]")
        {
            Style = Style.Parse("green"),
            Justification = Justify.Center
        };
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
        
        var statusPanel = new Panel($"[green]✅ Ready to cast![/] You can now control your {_state.SelectedDevice?.Model} device.")
            .BorderColor(Color.Green)
            .Padding(1, 0);
        
        AnsiConsole.Write(statusPanel);
        AnsiConsole.WriteLine();
    }

    public void DisplayDevicesTable()
    {
        var table = new Table();
        table.AddColumn("[blue]#[/]");
        table.AddColumn("[blue]Device Name[/]");
        table.AddColumn("[blue]Model[/]");
        table.AddColumn("[blue]Network Address[/]");

        for (int i = 0; i < _state.Devices.Count; i++)
        {
            var device = _state.Devices[i];
            table.AddRow(
                $"[yellow]{i + 1}[/]",
                $"[white]{device.Name}[/]",
                $"[cyan]{device.Model}[/]",
                $"[dim]{device.DeviceUri?.Host}[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public Table CreateDeviceStatusTable(ChromecastStatus status)
    {
        var table = new Table();
        table.AddColumn("[blue]Property[/]");
        table.AddColumn("[blue]Value[/]");
        
        table.AddRow("[cyan]Volume Level[/]", $"[white]{status.Volume?.Level:P0}[/]");
        table.AddRow("[cyan]Muted[/]", status.Volume?.Muted == true ? "[red]Yes[/]" : "[green]No[/]");
        table.AddRow("[cyan]Applications[/]", $"[white]{status.Applications?.Count ?? 0}[/]");
        
        if (status.Applications?.Any() == true)
        {
            table.AddEmptyRow();
            table.AddRow("[yellow]Running Applications:[/]", "");
            
            foreach (var app in status.Applications)
            {
                table.AddRow($"[dim]  • {app.DisplayName}[/]", $"[dim]{app.StatusText}[/]");
            }
        }
        
        return table;
    }
}