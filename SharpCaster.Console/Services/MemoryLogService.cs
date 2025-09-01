using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SharpCaster.Console.Services;

public class MemoryLogService
{
    private readonly ConcurrentQueue<LogEntry> _logs = new();
    private readonly int _maxLogs;

    public MemoryLogService(int maxLogs = 500)
    {
        _maxLogs = maxLogs;
    }

    public void AddLog(LogLevel level, string category, string message, Exception? exception = null)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Category = category,
            Message = message,
            Exception = exception
        };

        _logs.Enqueue(entry);
        
        while (_logs.Count > _maxLogs)
        {
            _logs.TryDequeue(out _);
        }
    }

    public IReadOnlyList<LogEntry> GetLogs()
    {
        return _logs.ToList();
    }

    public IReadOnlyList<LogEntry> GetLogs(LogLevel minLevel)
    {
        return _logs.Where(log => log.Level >= minLevel).ToList();
    }

    public IReadOnlyList<LogEntry> GetRecentLogs(int count)
    {
        return _logs.TakeLast(count).ToList();
    }

    public void ClearLogs()
    {
        while (_logs.TryDequeue(out _)) { }
    }

    public int Count => _logs.Count;
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }

    public string GetLevelDisplay()
    {
        return Level switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            LogLevel.None => "   ",
            _ => "UNK"
        };
    }

    public string GetLevelColor()
    {
        return Level switch
        {
            LogLevel.Trace => "dim",
            LogLevel.Debug => "blue",
            LogLevel.Information => "white",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "bold red",
            _ => "white"
        };
    }
}

public class MemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, MemoryLogger> _loggers = new();
    private readonly MemoryLogService _memoryLogService;

    public MemoryLoggerProvider(MemoryLogService memoryLogService)
    {
        _memoryLogService = memoryLogService;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new MemoryLogger(name, _memoryLogService));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public class MemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly MemoryLogService _memoryLogService;

    public MemoryLogger(string categoryName, MemoryLogService memoryLogService)
    {
        _categoryName = categoryName;
        _memoryLogService = memoryLogService;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Trace;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        _memoryLogService.AddLog(logLevel, _categoryName, message, exception);
    }
}