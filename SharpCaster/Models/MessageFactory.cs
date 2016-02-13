using System;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Models
{
    public static class MessageFactory
    {
        public static class DialConstants
        {
            public const string DialMultiScreenUrn = "urn:dial-multiscreen-org:service:dial:1";
            public const string DialConnectionUrn = "urn:x-cast:com.google.cast.tp.connection";
            public const string DialHeartbeatUrn = "urn:x-cast:com.google.cast.tp.heartbeat";
            public const string DialReceiverUrn = "urn:x-cast:com.google.cast.receiver";
            public const string DialMediaUrn = "urn:x-cast:com.google.cast.media";
        }

        private static readonly string UniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

        public static CastMessage Close => new CastMessage
        {
            Namespace = DialConstants.DialConnectionUrn,
            PayloadUtf8 = new CloseRequest().ToJson()
        };

        public static CastMessage Connect() => new CastMessage
        {
            Namespace = DialConstants.DialConnectionUrn,
            PayloadUtf8 = new ConnectRequest().ToJson()
        };

        public static CastMessage ConnectWithDestination(string destinationId) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DialConstants.DialConnectionUrn,
            PayloadUtf8 = new ConnectRequest().ToJson()
        };

        public static CastMessage Volume(float level) => new CastMessage
        {
            Namespace = DialConstants.DialReceiverUrn,
            PayloadUtf8 = new VolumeRequest(level).ToJson()
        };

        public static CastMessage Ping => new CastMessage
        {
            Namespace = DialConstants.DialHeartbeatUrn,
            PayloadUtf8 = new PingRequest().ToJson()
        };

        public static CastMessage Pong() => new CastMessage
        {
            Namespace = DialConstants.DialHeartbeatUrn,
            PayloadUtf8 = new PongRequest().ToJson()
        };

        public static CastMessage Status() => new CastMessage
        {
            Namespace = DialConstants.DialReceiverUrn,
            PayloadUtf8 = new GetStatusRequest().ToJson()
        };

        public static CastMessage Play(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DialConstants.DialMediaUrn,
            PayloadUtf8 = new PlayRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Pause(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            
            Namespace = DialConstants.DialMediaUrn,
            PayloadUtf8 = new PauseRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Launch(string appId) => new CastMessage
        {
            Namespace = DialConstants.DialReceiverUrn,
            PayloadUtf8 = new LaunchRequest(appId).ToJson()
        };

        public static CastMessage Load(string destinationId, string payload) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DialConstants.DialMediaUrn,
            PayloadUtf8 = payload
        };

        public static CastMessage Seek(string destinationId, long mediaSessionId, double seconds)
            => new CastMessage(destinationId, UniqueSourceID)
            {
                Namespace =  DialConstants.DialMediaUrn,
                PayloadUtf8 = new SeekRequest(mediaSessionId, seconds).ToJson()
            };
    }
}
