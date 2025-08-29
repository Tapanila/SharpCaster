using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using Sharpcaster.Test.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class MediaChannelTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact(Skip = "Skipped for autotesting because manual intervention on device is needed for this test!")]
        public async Task TestWaitForDeviceStopDuringPlayback()
        {
            //   To get this test Passing, you have to manually operate the used Chromecast device!
            //   I use it with a JBL speaker device. This device has 5 buttons. (ON/OFF, Vol-, Vol+, Play/Pause, (and WLAN-Connect))
            //   Vol+/- and Play/Pause do operate and trigger 'unasked' MediaStatusChanged events which work as designed.
            //
            //   Pressing the ON/OFF key during Playback causes the device to send:
            //       1. on media channel MediaStatus -> changed to 'Paused'
            //       2. on receiver channel a ReceiverStatus Message -> the applications array is omitted (set to NULL) here.
            //       3. on connection channel a close message.
            // 
            //   after the test media starts playing you have 20 seconds to press the device stop button. Then this should pass as green!
            //
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            if (fixture.Receivers[0].Model == "JBL Playlist")
            {
                var media = new Media
                {
                    ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
                };

                AutoResetEvent _disconnectReceived = new(false);
                MediaChannel mediaChannel = client.MediaChannel;

                mediaChannel.StatusChanged += (object sender, MediaStatus e) =>
                {
                        outputHelper.WriteLine(e.PlayerState.ToString());
                };

                client.Disconnected += (object sender, EventArgs e) =>
                {
                    try
                    {
                        _disconnectReceived.Set();
                        outputHelper.WriteLine("Disconnect received.");
                    }
                    catch (Exception)
                    {
                    }
                };

                MediaStatus status = await client.MediaChannel.LoadAsync(media);

                //This keeps the test running for 20 seconds or until the device initates the wanted stop-disconnect.
                Assert.True(_disconnectReceived.WaitOne(20000), "Have you manually stopped the device while playback? If you did so, this is a real Error :-) !");

                // To reuse the device now you have to create a new connection and reload the app ...
                client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
                status = await client.MediaChannel.LoadAsync(media);
                Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            }
            else
            {
                Assert.Fail("This test only runs with a 'JBL Playlist' device and also needs manual operations!");
            }
            await client.DisconnectAsync();
        }
        [Fact]
        public async Task TestMediaSupportedCommandsWithSingleMediaFile()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };
            MediaStatus status = await client.MediaChannel.LoadAsync(media);
            Assert.NotNull(status);
            Assert.Equal(
                MediaCommand.ALL_BASIC_MEDIA | MediaCommand.STREAM_TRANSFER,
                status.SupportedMediaCommands);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestMediaSupportedCommandsWithQueue()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            var testQueue = TestHelper.CreateTestCd;
            MediaStatus status = await client.MediaChannel.QueueLoadAsync(testQueue);
            Assert.NotNull(status);
            Assert.Equal(
                MediaCommand.ALL_BASIC_MEDIA | MediaCommand.STREAM_TRANSFER,
                status.SupportedMediaCommands);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadMediaQueueAndCheckContent()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            QueueItem[] MyCd = helper.TestHelper.CreateTestCd;

            MediaStatus status = await client.MediaChannel.QueueLoadAsync(MyCd);
            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2, status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            int[] ids = await client.MediaChannel.QueueGetItemIdsAsync();

            Assert.Equal(4, ids.Length);

            foreach (int id in ids)
            {
                QueueItem[] items = await client.MediaChannel.QueueGetItemsAsync([id]);
                Assert.Single(items);
            }

            QueueItem[] items2 = await client.MediaChannel.QueueGetItemsAsync(ids);
            Assert.Equal(4, items2.Length);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadingMediaQueue()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            QueueItem[] MyCd = helper.TestHelper.CreateTestCd;

            MediaStatus status = await client.MediaChannel.QueueLoadAsync(MyCd);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2, status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadingMediaQueueWithItemIds()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            AutoResetEvent _autoResetEvent = new(false);

            QueueItem[] MyCd = helper.TestHelper.CreateTestCdWithItemIds;
            // ItemId is Unique identifier of the item in the queue.
            // when doing QueueLoad or QueueInsert it must be null
            // (as it will be assigned by the receiver when an item is first created/inserted).
            // For other operations it is mandatory.
            // That's why this should fail

            client.MediaChannel.InvalidRequest += (object sender, InvalidRequestMessage e) =>
            {
                outputHelper.WriteLine("Invalid Request Error happened: " + e.Reason);
                _autoResetEvent.Set();
            };

            MediaStatus status = await client.MediaChannel.QueueLoadAsync(MyCd);

            Assert.True(_autoResetEvent.WaitOne(1000));

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadingMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.MediaChannel.LoadAsync(media);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task StartApplicationAThenStartBAndLoadMedia()
        {
            var th = new TestHelper();
            var client = await th.CreateAndConnectClient(outputHelper, fixture.Receivers[0]);

            _ = await client.LaunchApplicationAsync("233637DE", false);

            await client.DisconnectAsync();
            await client.ConnectChromecast(fixture.Receivers[0]);
            _ = await client.LaunchApplicationAsync("B3419EF5", false);
            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };
            _ = await client.MediaChannel.LoadAsync(media);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadingAndPausingMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            AutoResetEvent _autoResetEvent = new(false);

            await client.ReceiverChannel.StopApplication();
            await client.DisconnectAsync();
            client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            string runSequence = "R";
            bool firstPlay = true;

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.MediaChannel.StatusChanged += async (object sender, MediaStatus e) =>
            {
                //runSequence += ".";
                if (e.PlayerState == PlayerStateType.Playing)
                {
                    if (firstPlay)
                    {
                        firstPlay = false;
                        runSequence += "p";
                        mediaStatus = await client.MediaChannel.PauseAsync();
                        Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);
                        runSequence += "P";
                        _autoResetEvent.Set();
                    }
                }
            };

            runSequence += "1";
            mediaStatus = await client.MediaChannel.LoadAsync(media);
            await Task.Delay(10, Xunit.TestContext.Current.CancellationToken);
            runSequence += "2";

            Assert.True(_autoResetEvent.WaitOne(300));
            runSequence += "3";
            Assert.Equal("R1p2P3", runSequence);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestLoadingAndStoppingMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            AutoResetEvent _autoResetEvent = new(false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            bool firstPlay = true;

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.MediaChannel.StatusChanged += async (object sender, MediaStatus e) =>
            {
                try
                {
                    if (e.PlayerState == PlayerStateType.Playing)
                    {
                        if (firstPlay)
                        {
                            firstPlay = false;
                            await Task.Delay(2000); // Listen for some time
                            mediaStatus = await client.MediaChannel.StopAsync();
                            _autoResetEvent.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    outputHelper.WriteLine("Exception in Event Handler: " + ex.ToString());
                }
            };

            mediaStatus = await client.MediaChannel.LoadAsync(media);

            //This checks that within 5000 ms we have loaded video and were able to pause it
            await Task.Delay(3000, Xunit.TestContext.Current.CancellationToken);
            Assert.True(_autoResetEvent.WaitOne(2000));

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestFailingLoadMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = ""
            };

            LoadFailedMessage loadFailedMessage = null;

            client.MediaChannel.LoadFailed += (object sender, LoadFailedMessage e) =>
            {
                loadFailedMessage = e;
            };

            

            MediaStatus mediaStatus;
 
            mediaStatus = await client.MediaChannel.LoadAsync(media);

            await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);
            

            Assert.NotNull(loadFailedMessage);
        }

        [Fact]
        public async Task TestJoiningRunningMediaSessionAndPausingMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            await client.MediaChannel.LoadAsync(media);

            await client.MediaChannel.PlayAsync();

            await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);

            client = TestHelper.GetClientWithTestOutput(outputHelper);
            var status = await client.ConnectChromecast(fixture.Receivers[0]);

            var applicationRunning = status.Application;
            await client.LaunchApplicationAsync(applicationRunning.AppId, true);
            await client.MediaChannel.PauseAsync();
        }


        [Fact]
        public async Task TestRepeatingAllQueueMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Loping%20Sting.mp3"
            };

            var queueItem = new QueueItem
            {
                Media = media
            };

            await client.MediaChannel.QueueLoadAsync([queueItem], RepeatModeType.ALL);
            var test = await client.MediaChannel.PlayAsync();

            Assert.Equal(RepeatModeType.ALL, test.RepeatMode);
        }

        [Fact]
        public async Task TestRepeatingOffQueueMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Loping%20Sting.mp3"
            };

            var queueItem = new QueueItem
            {
                Media = media
            };

            await client.MediaChannel.QueueLoadAsync([queueItem], RepeatModeType.OFF);
            var test = await client.MediaChannel.PlayAsync();

            Assert.Equal(RepeatModeType.OFF, test.RepeatMode);
        }

        [Fact]
        public async Task TestRepeatingSingleQueueMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Loping%20Sting.mp3"
            };

            var queueItem = new QueueItem
            {
                Media = media
            };

            await client.MediaChannel.QueueLoadAsync([queueItem], RepeatModeType.SINGLE);
            var test = await client.MediaChannel.PlayAsync();

            Assert.Equal(RepeatModeType.SINGLE, test.RepeatMode);
        }

        [Fact]
        public async Task TestRepeatingAllAndShuffleQueueMedia()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Loping%20Sting.mp3"
            };

            var queueItem = new QueueItem
            {
                Media = media
            };

            await client.MediaChannel.QueueLoadAsync([queueItem], RepeatModeType.ALL_AND_SHUFFLE);
            var test = await client.MediaChannel.PlayAsync();

            Assert.Equal(RepeatModeType.ALL_AND_SHUFFLE, test.RepeatMode);
        }

        [Fact]
        public async Task TestFailingQueue()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            bool errorHappened = false;

            QueueItem[] MyCd = helper.TestHelper.CreateFailingQueue;


            client.MediaChannel.InvalidRequest += (object sender, InvalidRequestMessage e) =>
            {
                outputHelper.WriteLine("Invalid Request Error happened: " + e.Reason);
                errorHappened = true;
            };

            client.MediaChannel.LoadFailed += (object sender, LoadFailedMessage e) =>
            {
                outputHelper.WriteLine("Load Failed Error happened and failing media was  " + e.ItemId);
                errorHappened = true;
            };

            client.MediaChannel.ErrorHappened += (object sender, ErrorMessage e) =>
            {
                outputHelper.WriteLine("Error happened: " + e.ToString());
                errorHappened = true;
            };

            var result = await client.MediaChannel.QueueLoadAsync(MyCd);


            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);
            Assert.True(errorHappened);
        }

        [Fact]
        public async Task TestLoadingMediaWithSubtitles()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var subtitleTrack = new Track
            {
                TrackId = 1,
                Type = TrackType.TEXT,
                Subtype = TextTrackType.SUBTITLES,
                TrackContentId = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt",
                TrackContentType = "text/vtt",
                Name = "English Subtitles",
                Language = "en"
            };

            var style = new TextTrackStyle
            {
                EdgeColor = CastColors.Green, // Using predefined color
                ForegroundColor = CastColor.FromRgb(255, 255, 255), // White text
                BackgroundColor = CastColor.FromRgba(0, 0, 0, 128), // Semi-transparent black background  
                EdgeType = TextTrackEdgeType.OUTLINE,
                FontFamily = "sans-serif",
                FontGenericFamily = TextTrackFontGenericFamily.SANS_SERIF,
                FontScale = 1.0,
                FontStyle = TextTrackFontStyle.NORMAL,
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                ContentType = "video/mp4",
                Tracks = [subtitleTrack],
                TextTrackStyle = style
            };

            // Load media with active subtitle track
            MediaStatus status = await client.MediaChannel.LoadAsync(media, true, [subtitleTrack.TrackId]);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
            
            // Verify that the media has tracks
            var currentItem = status.Items[0];
            Assert.NotNull(currentItem.Media);
            Assert.NotNull(currentItem.Media.Tracks);
            Assert.Single(currentItem.Media.Tracks);
            
            var loadedTrack = currentItem.Media.Tracks[0];
            Assert.Equal(TrackType.TEXT, loadedTrack.Type);
            Assert.Equal(TextTrackType.SUBTITLES, loadedTrack.Subtype);
            Assert.Equal("English Subtitles", loadedTrack.Name);
            Assert.Equal("en", loadedTrack.Language);

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestSeekAsync()
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
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            // Seek to 30 seconds
            MediaStatus seekStatus = await client.MediaChannel.SeekAsync(30.0);
            Assert.True(seekStatus.CurrentTime >= 30.0, "Seek did not reach the expected time.");

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestSetPlaybackRateAsync()
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
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            // Set playback rate to 1.5x speed
            MediaStatus playbackRateStatus = await client.MediaChannel.SetPlaybackRateAsync(1.5);
            Assert.Equal(1.5, playbackRateStatus.PlaybackRate);

            await client.DisconnectAsync();
        }

        [Fact(Skip = "Not implemented")]
        public async Task TestSendUserActionAsync()
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
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            // Send a like user action
            var userAction = UserAction.DISLIKE;
            
            //await client.MediaChannel.SendUserActionAsync(userAction);

            var statusAfterAction = await client.MediaChannel.GetMediaStatusAsync();

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestEditTracksAsync()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var subtitleTrack = new Track
            {
                TrackId = 1,
                Type = TrackType.TEXT,
                Subtype = TextTrackType.SUBTITLES,
                TrackContentId = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt",
                TrackContentType = "text/vtt",
                Name = "English Subtitles",
                Language = "en"
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                ContentType = "video/mp4",
                Tracks = [subtitleTrack]
            };

            MediaStatus status = await client.MediaChannel.LoadAsync(media);
            Assert.Equal(PlayerStateType.Playing, status.PlayerState);

            // Wait a bit for media to start
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            MediaStatus editTracksStatus = await client.MediaChannel.EditTracksAsync([1]);
            Assert.Equal([1], editTracksStatus.ActiveTrackIds);

            await client.DisconnectAsync();
        }


        [Fact]
        public async Task TestGetMediaStatusAsync()
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
            await Task.Delay(2000, Xunit.TestContext.Current.CancellationToken);

            // Get current media status
            MediaStatus currentStatus = await client.MediaChannel.GetMediaStatusAsync();
            Assert.NotNull(currentStatus);
            Assert.True(currentStatus.MediaSessionId > 0);

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestVolumeWithMediaChannel()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            await client.MediaChannel.LoadAsync(media);

            var status = await client.MediaChannel.SetVolumeAsync(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            status = await client.MediaChannel.SetVolumeAsync(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.MediaChannel.SetVolumeAsync(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.MediaChannel.SetVolumeAsync(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);
            await client.DisconnectAsync();
        }
    }
}
