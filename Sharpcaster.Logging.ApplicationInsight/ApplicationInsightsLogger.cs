using Microsoft.ApplicationInsights;
using Serilog;
using System.Threading.Tasks;

namespace Sharpcaster.Logging.ApplicationInsight
{
    public class ApplicationInsightLogger
    {
        private ILogger _logger;
        private TelemetryClient _telemetryClient;
        public ApplicationInsightLogger()
        {
            _telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = "64d1e5b2-f91a-4d3c-9442-83e46a7b0e13"
            };
            _logger = new LoggerConfiguration()
                .WriteTo.ApplicationInsightsEvents(_telemetryClient)
                .CreateLogger();
 
            Log.Logger = _logger;
        }

        public async Task Flush()
        {
            _telemetryClient.Flush();
            await Task.Delay(1000);
        }
    }
}
