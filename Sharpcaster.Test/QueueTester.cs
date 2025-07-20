using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class QueueTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task TestLoadingMediaQueueAndNavigateNextPrev()
        {
            // Arrange - Setup test infrastructure
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;

            try
            {
                // Act - Load the queue and verify initial state
                MediaStatus initialStatus = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.Equal(initialStatus.Media.ContentId, testQueue[0].Media.ContentId);
                outputHelper.WriteLine("1st track started playing - Listening it for 1,5 seconds");
                await Task.Delay(1500, Xunit.TestContext.Current.CancellationToken); // Listen briefly
                var statusAfterNext = await client.MediaChannel.QueueNextAsync();
                Assert.Equal(statusAfterNext.Media.ContentId, testQueue[1].Media.ContentId);
                outputHelper.WriteLine("2nd track started playing - Listening it for 1,5 seconds");
                await Task.Delay(1500, Xunit.TestContext.Current.CancellationToken); // Listen briefly
                var statusAfterPrevious = await client.MediaChannel.QueuePrevAsync();
                Assert.Equal(initialStatus.Media.ContentId, testQueue[0].Media.ContentId);
            }
            finally
            {
                // Cleanup
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [Fact]
        public async Task TestLoadingMediaQueueAndClickingNextTillOutOfMedia()
        {
            // Arrange - Setup test infrastructure
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;

            try
            {
                // Load the queue and verify initial state
                MediaStatus mediaStatus = await client.MediaChannel.QueueLoadAsync(testQueue);
                //loop till out of media
                for (int i = 0; i < testQueue.Length; i++)
                {
                    outputHelper.WriteLine(i+ "/" + testQueue.Length);
                    mediaStatus = await client.MediaChannel.QueueNextAsync();
                }
                Assert.Null(mediaStatus);
            }
            finally
            {
                // Cleanup
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }
    }
}