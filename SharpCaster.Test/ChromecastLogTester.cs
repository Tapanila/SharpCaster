using Sharpcaster.Logging.ApplicationInsight;
using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastLogTester
    {
        [Fact]
        public async void TestingStuff()
        {
            var logger = new ApplicationInsightLogger();
            var client = new ChromecastClient();
            await logger.Flush();
        }
    }
}
