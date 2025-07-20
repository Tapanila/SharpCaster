using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;

namespace Sharpcaster.Test.Aot
{
    [TestClass]
    public class QueueTester
    {
        [TestMethod]
        public async Task LoadQueueThenInsertItem()
        {
            var receivers = await new MdnsChromecastLocator().FindReceiversAsync(TimeSpan.FromMilliseconds(500));
            if (receivers?.Any() != true) throw new Exception("No Chromecast devices found.");

            var client = new ChromecastClient();
            await client.ConnectChromecast(receivers.First());
            var status = await client.LaunchApplicationAsync("B3419EF5");
            if (status.Application?.AppId != "B3419EF5") throw new Exception("Failed to launch app.");

            var items = new[]
            {
                new QueueItem { Media = new Media { ContentId = "Aquarium", ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Aquarium.mp3", ContentType = "audio/mpeg" } },
                new QueueItem { Media = new Media { ContentId = "Arcane", ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Arcane.mp3", ContentType = "audio/mpeg" } },
                new QueueItem { Media = new Media { ContentId = "A Mission", ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/A%20Mission.mp3", ContentType = "audio/mpeg" } },
                new QueueItem { Media = new Media { ContentId = "All This", ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/All%20This.mp3", ContentType = "audio/mpeg" } }
            };

            var queueStatus = await client.MediaChannel.QueueLoadAsync(items);
            Assert.IsTrue(queueStatus?.PlayerState == PlayerStateType.Playing, "Queue not playing.");

            var newItem = new QueueItem { ItemId = items.Length + 1, Media = new Media { ContentUrl = "http://www.openmusicarchive.org/audio/Drunkards%20Special%20by%20Coley%20Jones.mp3" }, StartTime = 0 };
            var updateStatus = await client.MediaChannel.QueueInsertAsync([newItem]);
            Assert.IsTrue(updateStatus?.PlayerState == PlayerStateType.Playing, "Queue update not playing.");
        }
    }
}
