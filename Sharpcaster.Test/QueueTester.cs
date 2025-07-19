using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class QueueTester : IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public QueueTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestLoadingMediaQueueAndNavigateNextPrev(ChromecastReceiver receiver)
        {
            // Arrange - Setup test infrastructure
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(output, receiver);
            
            var testQueue = TestHelper.CreateTestCd;

            try
            {
                // Act - Load the queue and verify initial state
                MediaStatus initialStatus = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.Equal(PlayerStateType.Playing, initialStatus.PlayerState);
                Assert.Equal(initialStatus.Media.ContentId, testQueue[0].Media.ContentId);
                output.WriteLine("1st track started playing - Listening it for 1,5 seconds");
                await Task.Delay(1500); // Listen briefly
                var statusAfterNext = await client.MediaChannel.QueueNextAsync();
                Assert.Equal(statusAfterNext.Media.ContentId, testQueue[1].Media.ContentId);
                output.WriteLine("2nd track started playing - Listening it for 1,5 seconds");
                await Task.Delay(1500); // Listen briefly
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

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestLoadingMediaQueueAndClickingNextTillOutOfMedia(ChromecastReceiver receiver)
        {
            // Arrange - Setup test infrastructure
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(output, receiver);

            var testQueue = TestHelper.CreateTestCd;

            try
            {
                // Load the queue and verify initial state
                MediaStatus mediaStatus = await client.MediaChannel.QueueLoadAsync(testQueue);
                //loop till out of media
                for (int i = 0; i < testQueue.Length; i++)
                {
                    output.WriteLine(i+ "/" + testQueue.Length);
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