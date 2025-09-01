using Sharpcaster.Test.helper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class MemoryAllocationTester(ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task ConnectToChromecastAndLaunchApplication()
        {
            long memoryBefore = GC.GetTotalMemory(true);
            var TestHelper = new TestHelper();
            var client = new ChromecastClient();
            await client.ConnectChromecast(fixture.Receivers[0]);
            var status = await client.LaunchApplicationAsync("B3419EF5");

            Assert.Equal("B3419EF5", status.Application.AppId);

            await client.DisconnectAsync();

            long memoryAfter = GC.GetTotalMemory(true);
            long memoryUsed = memoryAfter - memoryBefore;
            double memoryUsedMB = memoryUsed / (1024.0 * 1024.0);
            Console.WriteLine($"Memory used: {memoryUsedMB:F2} MB");

            var memoryInfo = GC.GetGCMemoryInfo();
            var generations = memoryInfo.GenerationInfo;

            for (int i = 0; i < generations.Length; i++)
            {
                Console.WriteLine($"Generation {i}:");
                Console.WriteLine($"  Size Before GC: {generations[i].SizeBeforeBytes / (1024.0 * 1024.0):F2} MB");
                Console.WriteLine($"  Size After GC: {generations[i].SizeAfterBytes / (1024.0 * 1024.0):F2} MB");
            }
        }
    }
}
