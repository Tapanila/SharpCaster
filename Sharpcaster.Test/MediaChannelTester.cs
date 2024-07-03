﻿using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{

    [Collection("SingleCollection")]
    public class MediaChannelTester
    {
        private ITestOutputHelper output;
        public MediaChannelTester(ITestOutputHelper outputHelper) { 
            output = outputHelper;
        }

        [Fact]
        public async Task TestWaitForDeviceStopDuringPlayback() {
          
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
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);
            if (TestHelper.CurrentReceiver.Model == "JBL Playlist") {

                var media = new Media {
                    ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
                };

                AutoResetEvent _disconnectReceived = new AutoResetEvent(false);
                IMediaChannel mediaChannel = client.GetChannel<IMediaChannel>();

                mediaChannel.StatusChanged += (object sender, EventArgs e) => {
                    try {
                        MediaStatus status = mediaChannel.Status.FirstOrDefault();
                        output.WriteLine(status?.PlayerState.ToString());
                    } catch (Exception) {
                    }
                };

                client.Disconnected += (object sender, EventArgs e) => {
                    try {
                        _disconnectReceived.Set();
                        output.WriteLine("Disconnect received.");
                    } catch (Exception) {
                    }
                };

                MediaStatus status = await client.GetChannel<IMediaChannel>().LoadAsync(media);

                //This keeps the test running for 20 seconds or until the device initates the wanted stop-disconnect.
                Assert.True(_disconnectReceived.WaitOne(20000), "Have you manually stopped the device while playback? If you did so, this is a real Error :-) !");

                // To reuse the device now you have to create a new connection and reload the app ...
                client = await TestHelper.CreateConnectAndLoadAppClient(output);
                status = await client.GetChannel<IMediaChannel>().LoadAsync(media);
                Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            } else {
                Assert.Fail("This test only runs with a 'JBL Playlist' device and also needs manual operations!");
            }
        }

     
        [Fact]
        public async Task TestLoadingMediaQueueAndNavigateNextPrev() {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);

            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
            IMediaChannel mediaChannel = client.GetChannel<IMediaChannel>();
            QueueItem[] MyCd = TestHelper.CreateTestCd();

            int testSequenceCount = 0;

            //We are setting up an event to listen to status change. Because we don't know when the audio has started to play
            mediaChannel.StatusChanged += async (object sender, EventArgs e) => {
                try {
                    MediaStatus status = mediaChannel.Status.FirstOrDefault();
                    int currentItemId = status?.CurrentItemId ?? -1;
                   
                    if (currentItemId != -1 && status.PlayerState == PlayerStateType.Playing) {

                        if (status?.Items?.ToList()?.Where(i => i.ItemId == currentItemId).FirstOrDefault()?.Media?.ContentUrl?.Equals(MyCd[0].Media.ContentUrl) ?? false) {
                            if (testSequenceCount == 0) {
                                testSequenceCount++;
                                output.WriteLine("First Test Track started playin. listen for some seconds....");
                                await Task.Delay(6000);
                                output.WriteLine("Lets goto next item");
                                status = await mediaChannel.QueueNextAsync(status.MediaSessionId);
                                // Asserts
                                // ...
                            } else {
                                testSequenceCount++;
                                output.WriteLine("First Test Track started for the 2nd time. Stop and end the test");
                                await Task.Delay(1000);
                                status = await mediaChannel.StopAsync();
                                output.WriteLine("test Sequence finished");
                                _autoResetEvent.Set();
                            }

                        } else if (status?.Items?.ToList()?.Where(i => i.ItemId == currentItemId).FirstOrDefault()?.Media?.ContentUrl?.Equals(MyCd[1].Media.ContentUrl) ?? false) {
                            output.WriteLine("2nd Test Track started playin. listen for some seconds....");
                            testSequenceCount++;
                            await Task.Delay(6000);
                            output.WriteLine("Lets goto back to first one");
                            status = await mediaChannel.QueuePrevAsync(status.MediaSessionId);
                        }

                    }
                } catch (Exception ex) {
                    output?.WriteLine(ex.ToString());
                    Assert.Fail(ex.ToString());
                }
            };



            MediaStatus status = await client.GetChannel<IMediaChannel>().QueueLoadAsync(MyCd);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2, status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

            //This keeps the test running untill all eventhandler sequence steps are finished. If something goes wrong we get a very slow timeout here.
            Assert.True(_autoResetEvent.WaitOne(20000));

        }

        [Fact]
        public async Task TestLoadMediaQueueAndCheckContent() {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);

            QueueItem[] MyCd = TestHelper.CreateTestCd();

            MediaStatus status = await client.GetChannel<IMediaChannel>().QueueLoadAsync(MyCd);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2, status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

            await Task.Delay(2000);

            int[] ids = await client.GetChannel<IMediaChannel>().QueueGetItemIdsAsync(status.MediaSessionId);

            Assert.Equal(4, ids.Length);

            foreach (int id in ids) {
                Item[] items = await client.GetChannel<IMediaChannel>().QueueGetItemsAsync(status.MediaSessionId, new int[] {id});
                Assert.Single(items);
            }

            Item[] items2 = await client.GetChannel<IMediaChannel>().QueueGetItemsAsync(status.MediaSessionId, ids);
            Assert.Equal(4, items2.Length);
            
        }



        [Fact]
        public async Task TestLoadingMediaQueue() {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);

            QueueItem[] MyCd = TestHelper.CreateTestCd();

            MediaStatus status = await client.GetChannel<IMediaChannel>().QueueLoadAsync(MyCd);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2, status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
          
        }

        [Fact]
        public async Task TestLoadingMedia()
        {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
          
        }

        [Fact]
        public async Task StartApplicationAThenStartBAndLoadMedia()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = TestHelper.GetClientWithTestOutput(output);
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("A9BCCB7C", false);

            await client.DisconnectAsync();
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("B3419EF5", false);
            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };
            _ = await client.GetChannel<IMediaChannel>().LoadAsync(media);
        }

        [Fact]
        public async Task TestLoadingAndPausingMedia()
        {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            String runSequence = "R";
            bool firstPlay = true;

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.GetChannel<IMediaChannel>().StatusChanged += async (object sender, EventArgs e) =>
            {
                //runSequence += ".";
                if (client.GetChannel<IMediaChannel>().Status.FirstOrDefault()?.PlayerState == PlayerStateType.Playing)
                {
                    if (firstPlay) {
                        firstPlay = false;
                        runSequence += "p";
                        mediaStatus = await client.GetChannel<IMediaChannel>().PauseAsync();
                        Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);
                        runSequence += "P";
                        _autoResetEvent.Set();
                    }
                } 
            };

            runSequence += "1";
            mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);
            runSequence += "2";

            //This checks that within 5000 ms we have loaded video and were able to pause it
            Assert.True(_autoResetEvent.WaitOne(5000));
            runSequence += "3";

            Assert.Equal("R1p2P3", runSequence);
        }

        [Fact]
        public async Task TestLoadingAndStoppingMedia()
        {
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output);
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            bool firstPlay = true;

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.GetChannel<IMediaChannel>().StatusChanged += async (object sender, EventArgs e) =>
            {
                try {
                    if (client.GetChannel<IMediaChannel>().Status.FirstOrDefault()?.PlayerState == PlayerStateType.Playing) {
                        if (firstPlay) {
                            firstPlay = false;
                            await Task.Delay(2000); // Listen for some time
                            mediaStatus = await client.GetChannel<IMediaChannel>().StopAsync();
                            _autoResetEvent.Set();
                        }
                    }
                } catch (Exception ex) {
                    output.WriteLine("Exception in Event Handler: " + ex.ToString());   
                }
            };

            mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            //This checks that within 5000 ms we have loaded video and were able to pause it
            Assert.True(_autoResetEvent.WaitOne(5000));

        }
      
    }

}
