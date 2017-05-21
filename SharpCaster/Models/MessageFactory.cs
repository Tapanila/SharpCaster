using System;
using Extensions.Api.CastChannel;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Models
{
    public static class MessageFactory
    {

        private static readonly string UniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

        public static CastMessage Close => new CastMessage
        {
            PayloadUtf8 = new CloseRequest().ToJson()
        };

        public static CastMessage Connect() => new CastMessage
        {
            PayloadUtf8 = new ConnectRequest().ToJson()
        };

        public static CastMessage ConnectWithDestination(string destinationId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new ConnectRequest().ToJson()
        };

        public static CastMessage Volume(double level) => new CastMessage
        {
            PayloadUtf8 = new VolumeRequest(level).ToJson()
        };

        public static CastMessage Volume(bool muted) => new CastMessage
        {
            PayloadUtf8 = new VolumeRequest(muted).ToJson()
        };

        public static CastMessage Ping => new CastMessage
        {
            PayloadUtf8 = new PingRequest().ToJson()
        };

        public static CastMessage Pong() => new CastMessage
        {
            PayloadUtf8 = new PongRequest().ToJson()
        };

        public static CastMessage Status() => new CastMessage
        {
            PayloadUtf8 = new GetStatusRequest().ToJson()
        };

        public static CastMessage Play(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PlayRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Pause(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PauseRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Next(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new NextRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Previous(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PreviousRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Launch(string appId) => new CastMessage
        {
            PayloadUtf8 = new LaunchRequest(appId).ToJson()
        };

        public static CastMessage Load(string destinationId, string payload) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = payload
        };

        public static CastMessage Seek(string destinationId, long mediaSessionId, double seconds)
            => new CastMessage(destinationId, UniqueSourceID)
            {
                PayloadUtf8 = new SeekRequest(mediaSessionId, seconds).ToJson()
            };

        public static CastMessage StopApplication(string sessionId) => new CastMessage
        {
            PayloadUtf8 = new StopApplicationRequest(sessionId).ToJson()
        };

        public static CastMessage MediaStatus(string destinationId) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new MediaStatusRequest().ToJson()
        };

        public static CastMessage StopMedia(long mediaSessionId) => new CastMessage
        {
            PayloadUtf8 = new StopMediaRequest(mediaSessionId).ToJson()
        };


    }
}