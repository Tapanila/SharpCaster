using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SharpCaster.Console.Services;

public class LogViewerService
{
    private readonly MemoryLogService _memoryLogService;

    public LogViewerService(MemoryLogService memoryLogService)
    {
        _memoryLogService = memoryLogService;
    }

    public async Task ShowLogViewerAsync()
    {
        while (true)
        {
            var choices = new[]
            {
                "View all logs",
                "View recent logs (last 20)",
                "View by log level",
                "Search logs",
                "Clear logs",
                "Export logs",
                "Auto-refresh view",
                "Back to main menu"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Log Viewer Options[/]")
                    .PageSize(10)
                    .AddChoices(choices)
                    .UseConverter(choice => choice switch
                    {
                        "View all logs" => "📋 View all logs",
                        "View recent logs (last 20)" => "🕐 View recent logs (last 20)",
                        "View by log level" => "🎯 View by log level",
                        "Search logs" => "🔍 Search logs",
                        "Clear logs" => "🗑️  Clear logs",
                        "Export logs" => "💾 Export logs",
                        "Auto-refresh view" => "🔄 Auto-refresh view",
                        "Back to main menu" => "⬅️  Back to main menu",
                        _ => choice
                    }));

            switch (choice)
            {
                case "View all logs":
                    DisplayLogs(_memoryLogService.GetLogs());
                    break;
                case "View recent logs (last 20)":
                    DisplayLogs(_memoryLogService.GetRecentLogs(20));
                    break;
                case "View by log level":
                    await ViewByLogLevelAsync();
                    break;
                case "Search logs":
                    await SearchLogsAsync();
                    break;
                case "Clear logs":
                    await ClearLogsAsync();
                    break;
                case "Export logs":
                    await ExportLogsAsync();
                    break;
                case "Auto-refresh view":
                    await AutoRefreshViewAsync();
                    break;
                case "Back to main menu":
                    return;
            }

            if (choice != "Back to main menu" && choice != "Auto-refresh view")
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                System.Console.ReadKey(true);
            }
        }
    }

    private void DisplayLogs(IReadOnlyList<LogEntry> logs)
    {
        if (logs.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No logs to display.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn(new TableColumn("[bold]Time[/]").Width(19));
        table.AddColumn(new TableColumn("[bold]Level[/]").Width(5));
        table.AddColumn(new TableColumn("[bold]Category[/]").Width(30));
        table.AddColumn(new TableColumn("[bold]Message[/]"));

        foreach (var log in logs)
        {
            var timeStr = log.Timestamp.ToString("HH:mm:ss.fff");
            var levelStr = $"[{log.GetLevelColor()}]{log.GetLevelDisplay()}[/]";
            var categoryStr = TruncateString(log.Category, 28);
            var messageStr = log.Exception != null 
                ? $"{log.Message} [red]({log.Exception.GetType().Name})[/]"
                : log.Message;

            table.AddRow(timeStr, levelStr, categoryStr, Markup.Escape(messageStr));
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[dim]Showing {logs.Count} log entries[/]");
    }

    private async Task ViewByLogLevelAsync()
    {
        var levelChoices = new[]
        {
            "Debug and above",
            "Information and above", 
            "Warning and above",
            "Error and above",
            "Critical only"
        };

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select minimum log level:[/]")
                .AddChoices(levelChoices));

        var minLevel = choice switch
        {
            "Debug and above" => LogLevel.Debug,
            "Information and above" => LogLevel.Information,
            "Warning and above" => LogLevel.Warning,
            "Error and above" => LogLevel.Error,
            "Critical only" => LogLevel.Critical,
            _ => LogLevel.Information
        };

        var filteredLogs = _memoryLogService.GetLogs(minLevel);
        DisplayLogs(filteredLogs);
    }

    private async Task SearchLogsAsync()
    {
        var searchTerm = AnsiConsole.Ask<string>("[yellow]Enter search term:[/]");
        
        var allLogs = _memoryLogService.GetLogs();
        var filteredLogs = allLogs.Where(log => 
            log.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            log.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        AnsiConsole.MarkupLine($"[green]Found {filteredLogs.Count} logs matching '{searchTerm}'[/]");
        DisplayLogs(filteredLogs);
    }

    private async Task ClearLogsAsync()
    {
        if (AnsiConsole.Confirm("[yellow]Are you sure you want to clear all logs?[/]"))
        {
            _memoryLogService.ClearLogs();
            AnsiConsole.MarkupLine("[green]All logs cleared.[/]");
        }
    }

    private async Task ExportLogsAsync()
    {
        var fileName = AnsiConsole.Ask("[yellow]Enter filename (without extension):[/]", "sharpcaster-logs");
        var fullPath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.txt");

        try
        {
            var logs = _memoryLogService.GetLogs();
            var lines = logs.Select(log => 
                $"{log.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{log.GetLevelDisplay()}] {log.Category}: {log.Message}" +
                (log.Exception != null ? $" | Exception: {log.Exception}" : ""));

            await File.WriteAllLinesAsync(fullPath, lines);
            AnsiConsole.MarkupLine($"[green]Logs exported to: {fullPath}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to export logs: {ex.Message}[/]");
        }
    }

    private async Task AutoRefreshViewAsync()
    {
        AnsiConsole.MarkupLine("[yellow]Auto-refresh mode - Press any key to stop[/]");
        AnsiConsole.WriteLine();

        var cts = new CancellationTokenSource();
        var keyTask = Task.Run(() => { System.Console.ReadKey(true); cts.Cancel(); });

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold yellow]📊 Live Log View[/]");
                AnsiConsole.MarkupLine("[dim]Press any key to stop auto-refresh[/]");
                AnsiConsole.WriteLine();

                var recentLogs = _memoryLogService.GetRecentLogs(15);
                DisplayLogs(recentLogs);

                try
                {
                    await Task.Delay(2000, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        finally
        {
            cts.Cancel();
            await keyTask;
        }

        AnsiConsole.MarkupLine("[green]Auto-refresh stopped.[/]");
    }

    private static string TruncateString(string input, int maxLength)
    {
        if (input.Length <= maxLength)
            return input;
        
        return input.Substring(0, maxLength - 3) + "...";
    }
}