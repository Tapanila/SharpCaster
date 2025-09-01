using Google.Protobuf.WellKnownTypes;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{

    public class ComplicatedCasesTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task TestingVolumeRelatedIssue()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                ContentType = "video/mp4",
                Metadata = new MediaMetadata
                {
                    Title = "Designing for Google Cast",
                    Images = new[]
                    {
                        new Image
                        {
                            Url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/poster/DesigningForGoogleCast.jpg"
                        }
                    }
                },
            };

            await client.MediaChannel.LoadAsync(media);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            var mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);

            await client.MediaChannel.PauseAsync();

            await client.DisconnectAsync();

            await client.ConnectChromecast(fixture.Receivers.FirstOrDefault());

            await client.LaunchApplicationAsync("B3419EF5", true);

            await client.MediaChannel.PlayAsync();

            status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);


        }
        [Fact]
        public async Task TestingVolumeRelatedIssueWithMp3()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var media = new Media
            {
                ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Arcane.mp3",
                ContentType = "audio/mpeg",
                Metadata = new MusicTrackMetadata
                {
                    Title = "Arcane",
                },
            };

            await client.MediaChannel.LoadAsync(media);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            var mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);

            await client.MediaChannel.PauseAsync();

            await client.DisconnectAsync();

            await client.ConnectChromecast(fixture.Receivers.FirstOrDefault());

            await client.LaunchApplicationAsync("B3419EF5", true);

            await client.MediaChannel.PlayAsync();

            status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);


        }

        [Fact]
        public async Task TestingWithSwitchingRunningApplication()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                ContentType = "video/mp4",
                StreamType = StreamType.Buffered,
                Metadata = new MediaMetadata
                {
                    Title = "Designing for Google Cast",
                    Images = new[]
                    {
                        new Image
                        {
                            Url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/poster/DesigningForGoogleCast.jpg"
                        }
                    }
                },
            };

            await client.MediaChannel.LoadAsync(media);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            var mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);

            await client.DisconnectAsync();

            await client.ConnectChromecast(fixture.Receivers.FirstOrDefault());

            await client.LaunchApplicationAsync("CC1AD845");

            await client.MediaChannel.LoadAsync(media);

            status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            mediaStatus = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, mediaStatus.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);


        }

        [Fact]
        public async Task TestLaunchingFirstDifferentApplicationAndThenLaunchingDifferentWithoutDisconnect()
        {
            var TestHelper = new TestHelper();
            //This will start the default receiver application (B3419EF5)
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            await client.LaunchApplicationAsync("B3419EF5");

            //This should cause issues
            try { 
                await client.LaunchApplicationAsync("CC1AD845", false);
                Assert.Fail("Expected exception was not thrown.");
            } catch (Exception ex)
            {
                Assert.IsType<TaskCanceledException>(ex);
                Assert.Contains("Client disconnected before receiving response.", ex.Message);
            }


        }

        [Fact]
        public async Task TestLaunchingFirstDifferentApplicationAndThenLaunchingDifferentWithDisconnect()
        {
            var TestHelper = new TestHelper();
            //This will start the default receiver application (B3419EF5)
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            await client.LaunchApplicationAsync("B3419EF5");

            await client.DisconnectAsync();
            client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            //This should work fine with disconnect
            try
            {
                await client.LaunchApplicationAsync("CC1AD845", false);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception was not thrown.");
            }
        }

        [Fact]
        public async Task TestSlowStatusHandlerAsync()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            // Register malicious handler
            client.MediaChannel.StatusChanged += async (sender, args) => {
                await Task.Delay(10000);
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.MediaChannel.LoadAsync(media);
            Assert.Equal(PlayerStateType.Playing, status.PlayerState);

            // Wait a bit for media to start
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            // Get current media status
            MediaStatus currentStatus = await client.MediaChannel.GetMediaStatusAsync();
            Assert.NotNull(currentStatus);
            Assert.True(currentStatus.MediaSessionId > 0);

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestWaiting10SecondsAfterConnect()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.MediaChannel.LoadAsync(media);
            Assert.Equal(PlayerStateType.Playing, status.PlayerState);

            // Wait a bit for media to start
            await Task.Delay(10000, Xunit.TestContext.Current.CancellationToken);

            await client.MediaChannel.PauseAsync();

            // Get current media status
            MediaStatus currentStatus = await client.MediaChannel.GetMediaStatusAsync();
            Assert.NotNull(currentStatus);
            Assert.True(currentStatus.MediaSessionId > 0);

            await client.DisconnectAsync();
        }
    }
}
