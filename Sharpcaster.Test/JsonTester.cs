using Microsoft.VisualBasic;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Media;
using System.Linq;
using System.Text.Json;
using Xunit;
using Sharpcaster.Test.helper;

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
            Assert.Equal("{\"requestId\":" + requestId +  ",\"type\":\"CONNECT\"}", output);
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
