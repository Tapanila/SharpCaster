using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Sharpcaster.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sharpcaster.Logging.ApplicationInsight
{
    public class ApplicationInsightLogger : ILogger
    {
        private TelemetryClient _telemetryClient;
        public ApplicationInsightLogger()
        {
            Initialize(new TelemetryConfiguration("64d1e5b2-f91a-4d3c-9442-83e46a7b0e13"));
        }

        public ApplicationInsightLogger(TelemetryConfiguration telemetryConfiguration)
        {
            Initialize(telemetryConfiguration);
        }

        private void Initialize(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
        }
        public async Task Log(string message)
        {
            _telemetryClient.TrackEvent(message);
        }
    }
}
