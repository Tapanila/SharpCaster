using System;
using Extensions.Api.CastChannel;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Models
{
    public static class MessageFactory
    {     
        /// <summary>
        /// Only change this value if you have a good reason to do it
        /// </summary>
        public static string UniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

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

        public static CastMessage Volume(double level, int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new VolumeRequest(level, requestId).ToJson()
        };

        public static CastMessage Volume(bool muted, int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new VolumeRequest(muted, requestId).ToJson()
        };

        public static CastMessage Ping => new CastMessage
        {
            PayloadUtf8 = new PingRequest().ToJson()
        };

        public static CastMessage Pong() => new CastMessage
        {
            PayloadUtf8 = new PongRequest().ToJson()
        };

        public static CastMessage Status(int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new GetStatusRequest(requestId).ToJson()
        };

        public static CastMessage Play(string destinationId, long mediaSessionId, int? requestId = null) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PlayRequest(mediaSessionId, requestId).ToJson()
        };

        public static CastMessage Pause(string destinationId, long mediaSessionId, int? requestId = null) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PauseRequest(mediaSessionId, requestId).ToJson()
        };

        public static CastMessage Next(string destinationId, long mediaSessionId,int? requestId = null) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new NextRequest(mediaSessionId, requestId).ToJson()
        };

        public static CastMessage Previous(string destinationId, long mediaSessionId, int? requestId = null) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new PreviousRequest(mediaSessionId,requestId).ToJson()
        };

        public static CastMessage Launch(string appId, int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new LaunchRequest(appId,requestId).ToJson()
        };

        public static CastMessage Load(string destinationId, string payload) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = payload
        };

        public static CastMessage Seek(string destinationId, long mediaSessionId, double seconds, int? requestId = null)
            => new CastMessage(destinationId, UniqueSourceID)
            {
                PayloadUtf8 = new SeekRequest(mediaSessionId, seconds, requestId).ToJson()
            };

        public static CastMessage StopApplication(string sessionId, int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new StopApplicationRequest(sessionId,requestId).ToJson()
        };

        public static CastMessage MediaStatus(string destinationId, int? requestId = null) => new CastMessage(destinationId, UniqueSourceID)
        {
            PayloadUtf8 = new MediaStatusRequest(requestId).ToJson()
        };

        public static CastMessage StopMedia(long mediaSessionId, int? requestId = null) => new CastMessage
        {
            PayloadUtf8 = new StopMediaRequest(mediaSessionId, requestId).ToJson()
        };


    }
}