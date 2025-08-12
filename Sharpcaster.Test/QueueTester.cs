using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using Sharpcaster.Test.helper;
using System.Linq;
using System.Threading.Tasks;
using xRetry.v3;
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

        [Fact]
        public async Task TestQueueInsertAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var initialQueue = TestHelper.CreateTestCd.Take(2).ToArray();
            
            try
            {
                // Load initial queue with 2 items
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(initialQueue);
                Assert.NotNull(status);
                
                await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);

                // Create new item to insert
                var newItem = new QueueItem
                {
                    Media = new Media
                    {
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Cheery%20Monday.mp3",
                        ContentType = "audio/mp3",
                        StreamType = StreamType.Buffered,
                        Metadata = new MusicTrackMetadata
                        {
                            Title = "Inserted Track",
                            Artist = "Test Artist"
                        }
                    }
                };

                // Insert at position 1 (second position)
                MediaStatus insertStatus = await client.MediaChannel.QueueInsertAsync([newItem], 1);
                Assert.NotNull(insertStatus);
            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [Fact]
        public async Task TestQueueRemoveAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;
            try { 
                // Load queue
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.NotNull(status);
                
                await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);

                // Get all item IDs
                int[] itemIds = await client.MediaChannel.QueueGetItemIdsAsync();
                Assert.Equal(4, itemIds.Length);

                // Remove the last item
                MediaStatus removeStatus = await client.MediaChannel.QueueRemoveAsync([itemIds[^1]]);
                itemIds = await client.MediaChannel.QueueGetItemIdsAsync();
                Assert.Equal(3, itemIds.Length);
            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [Fact]
        public async Task TestQueueReorderAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;
            
            try
            {
                // Load queue
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.NotNull(status);
                
                await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);

                // Get all item IDs
                int[] itemIds = await client.MediaChannel.QueueGetItemIdsAsync();
                Assert.True(itemIds.Length >= 2);

                var firstItemId = itemIds[0];
                var secondItemId = itemIds[1];
                var thirdItemId = itemIds[2];

                // Reorder: move first item to position 2
                MediaStatus reorderStatus = await client.MediaChannel.QueueReorderAsync([secondItemId, firstItemId], thirdItemId);
                itemIds = await client.MediaChannel.QueueGetItemIdsAsync();
                Assert.Equal(itemIds[1], firstItemId); // First item should now be at position 2
                Assert.Equal(itemIds[0], secondItemId); // Second item should now be at position 1
            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [Fact]
        public async Task TestQueueUpdateAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;
            
            try
            {
                // Load queue
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.NotNull(status);
                
                await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);

                // Get existing items and modify them
                int[] itemIds = await client.MediaChannel.QueueGetItemIdsAsync();
                
                QueueItem[] existingItems = await client.MediaChannel.QueueGetItemsAsync([itemIds[0]]);

                // Update the first item's metadata
                existingItems[0].Media.Metadata = new MusicTrackMetadata
                {
                    Title = "Updated Track Title"
                };

                MediaStatus updateStatus = await client.MediaChannel.QueueUpdateAsync(existingItems);
                Assert.Equal("Updated Track Title", updateStatus.Items[0].Media.Metadata.Title);

            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [RetryFact]
        public async Task TestQueueShuffleAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;
            
            try
            {
                // Load queue
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
                await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);
                int[] itemIdsAfterLoad = await client.MediaChannel.QueueGetItemIdsAsync();

                await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);

                // Enable shuffle
                MediaStatus shuffleStatus = await client.MediaChannel.QueueShuffleAsync(true);
                await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);
                int[] itemIdsAfterShuffle = await client.MediaChannel.QueueGetItemIdsAsync();
                await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);

                // Disable shuffle
                MediaStatus noShuffleStatus = await client.MediaChannel.QueueShuffleAsync(false);
                await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);
                int[] itemIdsAfterNoShuffle = await client.MediaChannel.QueueGetItemIdsAsync();

                Assert.NotEqual(itemIdsAfterLoad, itemIdsAfterShuffle);
                Assert.Equal(itemIdsAfterLoad, itemIdsAfterNoShuffle);
            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }

        [Fact]
        public async Task TestQueueSetRepeatModeAsync()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var testQueue = TestHelper.CreateTestCd;
            
            try
            {
                // Load queue
                MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
                Assert.NotNull(status);
                
                await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);

                // Test different repeat modes
                MediaStatus repeatAllStatus = await client.MediaChannel.QueueSetRepeatModeAsync(RepeatModeType.ALL);
                Assert.Equal(RepeatModeType.ALL, repeatAllStatus.RepeatMode);

                MediaStatus repeatSingleStatus = await client.MediaChannel.QueueSetRepeatModeAsync(RepeatModeType.SINGLE);
                Assert.Equal(RepeatModeType.SINGLE, repeatSingleStatus.RepeatMode);

                MediaStatus repeatOffStatus = await client.MediaChannel.QueueSetRepeatModeAsync(RepeatModeType.OFF);
                Assert.Equal(RepeatModeType.OFF, repeatOffStatus.RepeatMode);
            }
            finally
            {
                await client.ReceiverChannel.StopApplication();
                await client.DisconnectAsync();
            }
        }
    }
}