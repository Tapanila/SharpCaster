using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Media;
using System.Linq;
using System.Text.Json;
using Xunit;
using Sharpcaster.Messages.Receiver;

namespace Sharpcaster.Test
{
    public class JsonTester()
    {
        [Fact]
        public void TestConnectMessage()
        {
            IMessage connectMessage = new ConnectMessage();
            var requestId = (connectMessage as IMessageWithId).RequestId;

            var output = JsonSerializer.Serialize(connectMessage, SharpcasteSerializationContext.Default.ConnectMessage);
            Assert.Equal("{\"requestId\":" + requestId + ",\"type\":\"CONNECT\"}", output);
        }

        [Fact]
        public void TestSpotifyMediaChannelMessage()
        {
            const string spotifyMessageJson = """
            {
                "type": "MEDIA_STATUS",
                "status": [
                    {
                        "mediaSessionId": 1,
                        "playbackRate": 1,
                        "playerState": "PLAYING",
                        "currentTime": 148.858,
                        "supportedMediaCommands": 1039823,
                        "volume": {
                            "level": 1,
                            "muted": false
                        },
                        "activeTrackIds": [],
                        "media": {
                            "contentId": "spotify:track:1H7wITTESh5yB1KmrLhu5E",
                            "streamType": "BUFFERED",
                            "mediaCategory": "AUDIO",
                            "contentType": "application/x-spotify.track",
                            "metadata": {
                                "metadataType": 3,
                                "title": "Aquarius",
                                "songName": "Aquarius",
                                "artist": "Yoel Lewis",
                                "albumName": "Aquarius",
                                "images": [
                                    {
                                        "url": "https://i.scdn.co/image/ab67616d0000b2738a01f069dbcb3a1efb3ae1bd",
                                        "height": 640,
                                        "width": 640
                                    },
                                    {
                                        "url": "https://i.scdn.co/image/ab67616d000048518a01f069dbcb3a1efb3ae1bd",
                                        "height": 64,
                                        "width": 64
                                    },
                                    {
                                        "url": "https://i.scdn.co/image/ab67616d00001e028a01f069dbcb3a1efb3ae1bd",
                                        "height": 300,
                                        "width": 300
                                    }
                                ]
                            },
                            "entity": "spotify:track:1H7wITTESh5yB1KmrLhu5E",
                            "duration": 205,
                            "customData": {
                                "playerPlaybackState": {
                                    "timestamp": 1756489326468,
                                    "context": {
                                        "uri": "spotify:playlist:37i9dQZEVXcQRVmYV2sv97",
                                        "metadata": {
                                            "enhanced_context": "false",
                                            "context_description": "Discover Weekly"
                                        }
                                    },
                                    "position": 0,
                                    "duration": 205074,
                                    "paused": false,
                                    "playback_quality": "VERY_HIGH",
                                    "playback_features": {
                                        "hifi_status": "NONE",
                                        "playback_speed": {
                                            "current": 1,
                                            "selected": 1,
                                            "restricted": true
                                        },
                                        "signal_ids": [],
                                        "modes": {
                                            "context_enhancement": "NONE",
                                            "media": "audio",
                                            "jam": "off"
                                        }
                                    },
                                    "shuffle": false,
                                    "shuffle_mode": 0,
                                    "repeat_mode": 0
                                }
                            }
                        }
                    }
                ],
                "requestId": 1830132622
            }
            """;

            MediaStatusMessage mediaStatusMessage = JsonSerializer.Deserialize(spotifyMessageJson, SharpcasteSerializationContext.Default.MediaStatusMessage);
            Assert.NotNull(mediaStatusMessage);
            var status = mediaStatusMessage.Status.First();
            Assert.Equal(StreamType.Buffered, status.Media.StreamType);
            Assert.Equal("spotify:track:1H7wITTESh5yB1KmrLhu5E", status.Media.ContentId);
            Assert.Equal("Aquarius", status.Media.Metadata.Title);
            Assert.Equal(MetadataType.Music, status.Media.Metadata.MetadataType);
            var music = Assert.IsType<MusicTrackMetadata>(status.Media.Metadata);
            Assert.Equal("Yoel Lewis", music.Artist);
            Assert.Equal("Aquarius", music.AlbumName);
            Assert.Equal("application/x-spotify.track", status.Media.ContentType);
            Assert.Equal(205, status.Media.Duration);

        }

        [Fact]
        public void TestMediaStatusWithGermanUmlautTitle()
        {
            // Test parsing a MEDIA_STATUS message with German umlaut characters in the title
            var mediaStatusJson = """
            {
                "type": "MEDIA_STATUS",
                "status": [{
                    "mediaSessionId": 1,
                    "playbackRate": 1,
                    "playerState": "PLAYING",
                    "currentTime": 80288.353693,
                    "supportedMediaCommands": 274447,
                    "volume": {"level": 1, "muted": false},
                    "activeTrackIds": [],
                    "media": {
                        "contentId": "https://orf-live.ors-shoutcast.at/oe1-q1a",
                        "streamType": 1,
                        "contentType": "audio/mp4",
                        "metadata": {
                            "metadataType": 0,
                            "title": "Ö1-(128bps)"
                        },
                        "duration": null,
                        "mediaCategory": "AUDIO",
                        "tracks": [],
                        "breakClips": [],
                        "breaks": []
                    },
                    "currentItemId": 1,
                    "items": [{
                        "itemId": 1,
                        "media": {
                            "contentId": "https://orf-live.ors-shoutcast.at/oe1-q1a",
                            "streamType": 1,
                            "contentType": "audio/mp4",
                            "metadata": {
                                "metadataType": 0,
                                "title": "Ö1-(128bps)"
                            },
                            "duration": null,
                            "mediaCategory": "AUDIO"
                        },
                        "autoplay": true,
                        "orderId": 0
                    }],
                    "repeatMode": "REPEAT_OFF"
                }],
                "requestId": 1345463442
            }
            """;

            // Parse the JSON message
            MediaStatusMessage mediaStatusMessage = JsonSerializer.Deserialize(mediaStatusJson, SharpcasteSerializationContext.Default.MediaStatusMessage);

            // Verify the message was parsed correctly
            Assert.NotNull(mediaStatusMessage);
            Assert.NotNull(mediaStatusMessage.Status);
            Assert.Single(mediaStatusMessage.Status);

            var status = mediaStatusMessage.Status.First();
            Assert.Equal(1, status.MediaSessionId);
            Assert.Equal(PlayerStateType.Playing, status.PlayerState);

            // Verify the German umlaut title is preserved correctly
            Assert.NotNull(status.Media);
            Assert.NotNull(status.Media.Metadata);
            Assert.Equal("Ö1-(128bps)", status.Media.Metadata.Title);

            // Verify queue item also has correct title
            Assert.NotNull(status.Items);
            Assert.Single(status.Items);
            Assert.NotNull(status.Items[0].Media);
            Assert.NotNull(status.Items[0].Media.Metadata);
            Assert.Equal("Ö1-(128bps)", status.Items[0].Media.Metadata.Title);

            // Verify live stream properties
            Assert.Equal("https://orf-live.ors-shoutcast.at/oe1-q1a", status.Media.ContentId);
            Assert.Equal(StreamType.Live, status.Media.StreamType);
            Assert.Equal("audio/mp4", status.Media.ContentType);
            Assert.Null(status.Media.Duration);
        }
    }
}
