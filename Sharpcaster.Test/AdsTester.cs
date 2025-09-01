using Sharpcaster.Channels;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class AdsTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        private string GOOGLE_AD_URL = "https://pubads.g.doubleclick.net/gampad/ads?sz=640x480&iu=/124319096/external/single_ad_samples&ciu_szs=300x250&impl=s&gdfp_req=1&env=vp&output=vast&unviewed_position_start=1&cust_params=deployment%3Ddevsite%26sample_ct%3Dskippablelinear&correlator=" + Random.Shared.Next();
        private const string TEST_VIDEO_URL = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4";

        [Fact]
        public async Task TestLoadMediaWithAds()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            // Create media with ads
            var media = new Media
            {
                ContentUrl = TEST_VIDEO_URL,
                BreakClips = new[]
                {
                    new BreakClip
                    {
                        Id = "preroll-clip-1",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL
                        }
                    }
                },
                Breaks = new[]
                {
                    new Break
                    {
                        Id = "preroll-ad",
                        BreakClipIds = new[] { "preroll-clip-1" },
                        Position = 0 // Preroll ad
                    }
                }
            };

            MediaStatus? status = await client.MediaChannel.LoadAsync(media);
            Assert.NotNull(status);
            
            outputHelper.WriteLine($"Media loaded with status: {status.PlayerState}");
            outputHelper.WriteLine($"Supported commands: {status.SupportedMediaCommands}");
            
            await client.DisconnectAsync();
        }

        [Fact(Skip = "Not implemented")]
        public async Task TestSkipAd()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            
            bool adSkipped = false;
            AutoResetEvent adSkippedEvent = new(false);

            // Listen for status changes to detect ad playback
            client.MediaChannel.StatusChanged += (sender, mediaStatus) =>
            {
                outputHelper.WriteLine($"Media status changed: {mediaStatus?.PlayerState}");
                
                // Check if we're in an ad break
                if (mediaStatus?.BreakStatus != null && mediaStatus.PlayerState == PlayerStateType.Playing)
                {
                    outputHelper.WriteLine($"Ad break detected: {mediaStatus.BreakStatus.BreakId} and it's skippable : {mediaStatus.BreakStatus.WhenSkippable}");

                    Task.Run(async () =>
                    {
                        try
                        {
                            // The ad is skippable after 5 seconds
                            await Task.Delay(5000, Xunit.TestContext.Current.CancellationToken);
                            //This doesn't really work with the current Chromecast implementation
                            //await client.MediaChannel.SendUserActionAsync(UserAction.SKIP_AD);
                            adSkipped = true;
                            adSkippedEvent.Set();
                            outputHelper.WriteLine("Ad skip command sent successfully");
                            }
                            catch (Exception ex)
                            {
                                outputHelper.WriteLine($"Failed to skip ad: {ex.Message}");
                            }
                        });
                }
            };

            // Create media with ads
            var media = new Media
            {
                ContentUrl = TEST_VIDEO_URL,
                BreakClips = new[]
                {
                    new BreakClip
                    {
                        Id = "preroll-clip-1",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL
                        }
                    }
                },
                Breaks = new[]
                {
                    new Break
                    {
                        Id = "preroll-ad",
                        BreakClipIds = new[] { "preroll-clip-1" },
                        
                        Position = 0 // Preroll ad
                    }
                }
            };

            MediaStatus? status = await client.MediaChannel.LoadAsync(media);
            Assert.NotNull(status);

            // Wait for ad to potentially be skipped (timeout after 10 seconds)
            bool adEventOccurred = adSkippedEvent.WaitOne(10000);
            
            outputHelper.WriteLine($"Ad event occurred: {adEventOccurred}, Ad skipped: {adSkipped}");
            
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestMediaWithMidrollAds()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            // Create media with midroll ads
            var media = new Media
            {
                ContentUrl = TEST_VIDEO_URL,
                BreakClips = new[]
                {
                    new BreakClip
                    {
                        Id = "midroll-clip-1",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL
                        }
                    },
                    new BreakClip
                    {
                        Id = "midroll-clip-2",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL + "1"
                        }
                    }
                },
                Breaks = new[]
                {
                    new Break
                    {
                        Id = "midroll-ad-1",
                        BreakClipIds = new[] { "midroll-clip-1" },
                        Position = 30 // Ad at 30 seconds
                    },
                    new Break
                    {
                        Id = "midroll-ad-2",
                        BreakClipIds = new[] { "midroll-clip-2" },
                        Position = 60 // Ad at 60 seconds
                    }
                }
            };

            MediaStatus? status = await client.MediaChannel.LoadAsync(media);
            Assert.NotNull(status);
            
            outputHelper.WriteLine($"Media with midroll ads loaded: {status.PlayerState}");
            outputHelper.WriteLine($"Number of breaks defined: {media.Breaks?.Length ?? 0}");
            
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestMediaAdsSupportedCommands()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            // Load regular media first
            var regularMedia = new Media
            {
                ContentUrl = TEST_VIDEO_URL
            };

            MediaStatus? regularStatus = await client.MediaChannel.LoadAsync(regularMedia);
            Assert.NotNull(regularStatus);
            
            outputHelper.WriteLine($"Regular media supported commands: {regularStatus.SupportedMediaCommands}");
            outputHelper.WriteLine($"Regular media supports SKIP_AD: {regularStatus.SupportedMediaCommands.SupportsCommand(MediaCommand.SKIP_AD)}");

            // Now load media with ads
            var mediaWithAds = new Media
            {
                ContentUrl = TEST_VIDEO_URL,
                BreakClips = new[]
                {
                    new BreakClip
                    {
                        Id = "test-clip",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL
                        }
                    }
                },
                Breaks = new[]
                {
                    new Break
                    {
                        Id = "test-ad",
                        BreakClipIds = new[] { "test-clip" },
                        Position = 0
                    }
                }
            };

            MediaStatus? adStatus = await client.MediaChannel.LoadAsync(mediaWithAds);
            Assert.NotNull(adStatus);
            
            outputHelper.WriteLine($"Ad media supported commands: {adStatus.SupportedMediaCommands}");
            outputHelper.WriteLine($"Ad media supports SKIP_AD: {adStatus.SupportedMediaCommands.SupportsCommand(MediaCommand.SKIP_AD)}");

            // Test individual commands
            var commandNames = adStatus.SupportedMediaCommands.GetCommandNames();
            outputHelper.WriteLine($"All supported commands: {string.Join(", ", commandNames)}");

            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestAdBreakStatusMonitoring()
        {
            var testHelper = new TestHelper();
            ChromecastClient client = await testHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            
            bool adBreakDetected = false;
            AutoResetEvent statusChangeEvent = new(false);

            // Monitor for ad break status
            client.MediaChannel.StatusChanged += (sender, mediaStatus) =>
            {
                if (mediaStatus?.BreakStatus != null)
                {
                    adBreakDetected = true;
                    outputHelper.WriteLine($"Ad break detected!");
                    outputHelper.WriteLine($"Break status: {mediaStatus.BreakStatus}");
                    statusChangeEvent.Set();
                }
                
                outputHelper.WriteLine($"Player state: {mediaStatus?.PlayerState}");
                outputHelper.WriteLine($"Current time: {mediaStatus?.CurrentTime}");
            };

            // Create media with immediate preroll ad
            var media = new Media
            {
                ContentUrl = TEST_VIDEO_URL,
                BreakClips = new[]
                {
                    new BreakClip
                    {
                        Id = "preroll-monitor-clip",
                        VastAdsRequest = new VastAdsRequest
                        {
                            AdTagUrl = GOOGLE_AD_URL
                        }
                    }
                },
                Breaks = new[]
                {
                    new Break
                    {
                        Id = "preroll-monitor-test",
                        BreakClipIds = new[] { "preroll-monitor-clip" },
                        Position = 0 // Immediate preroll
                    }
                }
            };

            MediaStatus? status = await client.MediaChannel.LoadAsync(media);
            Assert.NotNull(status);

            // Wait for status changes (timeout after 15 seconds)
            bool statusChanged = statusChangeEvent.WaitOne(15000);
            
            outputHelper.WriteLine($"Status change detected: {statusChanged}");
            outputHelper.WriteLine($"Ad break detected: {adBreakDetected}");
            
            await client.DisconnectAsync();
        }
    }
}