using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharpcaster;
using SharpCaster.Console.Controllers;
using SharpCaster.Console.Flows;
using SharpCaster.Console.Models;
using SharpCaster.Console.Services;
using SharpCaster.Console.UI;

namespace SharpCaster.Console;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Ensure console supports UTF-8 for emojis
        System.Console.OutputEncoding = Encoding.UTF8;
        System.Console.InputEncoding = Encoding.UTF8;

        // Parse command line arguments
        var commandLineArgs = CommandLineParser.Parse(args);

        // Setup dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        var serviceProvider = services.BuildServiceProvider();

        // Check if this is command-line mode or interactive mode
        if (!commandLineArgs.IsInteractive || commandLineArgs.ShowHelp || commandLineArgs.ShowDevices || commandLineArgs.ShowVersion)
        {
            // Command-line mode
            var commandExecutor = serviceProvider.GetRequiredService<CommandExecutor>();
            return await commandExecutor.ExecuteCommandAsync(commandLineArgs);
        }
        else
        {
            // Interactive mode
            var app = serviceProvider.GetRequiredService<SharpCasterApplication>();
            await app.RunAsync();
            return 0;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register memory log service first
        services.AddSingleton<MemoryLogService>();
        services.AddSingleton<LogViewerService>();
        
        // Add logging with memory provider
        services.AddLogging(builder =>
        {
            builder.AddDebug().SetMinimumLevel(LogLevel.Trace);
            builder.Services.AddSingleton<ILoggerProvider>(provider =>
                new MemoryLoggerProvider(provider.GetRequiredService<MemoryLogService>()));
        });
        
        // Register application state as singleton
        services.AddSingleton<ApplicationState>();
        
        // Register device locator
        services.AddSingleton<MdnsChromecastLocator>();
        
        // Register UI helper
        services.AddSingleton<UIHelper>();
        
        // Register services
        services.AddSingleton<DeviceService>();
        services.AddSingleton<CommandExecutor>();
        
        // Register controllers
        services.AddSingleton<MediaController>();
        services.AddSingleton<QueueController>();
        
        // Register flows
        services.AddSingleton<ApplicationFlows>();
        
        // Register main application
        services.AddSingleton<SharpCasterApplication>();
    }
}

public class SharpCasterApplication
{
    private readonly ApplicationState _state;
    private readonly ApplicationFlows _flows;
    private readonly UIHelper _ui;
    private readonly ILogger<SharpCasterApplication> _logger;

    public SharpCasterApplication(
        ApplicationState state,
        ApplicationFlows flows,
        UIHelper ui,
        MdnsChromecastLocator locator,
        ILogger<SharpCasterApplication> logger)
    {
        _state = state;
        _flows = flows;
        _ui = ui;
        _logger = logger;
        
        // Initialize application state
        _state.Logger = logger;
        _state.Locator = locator;
    }

    public async Task RunAsync()
    {
        try
        {
            // Log application startup
            _logger.LogInformation("SharpCaster Console application starting");
            
            // Show welcome message
            _ui.ShowWelcome();
            
            // Step 1: Initial device discovery
            await _flows.InitialDiscoveryFlowAsync();
            
            // Step 2: Device selection and connection
            await _flows.DeviceSelectionFlowAsync();
            
            // Step 3: Main application flow with full options
            await _flows.MainApplicationFlowAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application error occurred");
            System.Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}