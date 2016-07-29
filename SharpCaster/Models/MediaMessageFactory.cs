using System;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Models
{
    public static class DefaultMediaMessageFactory
    {
        private const string DefaultMediaUrn = "urn:x-cast:com.google.cast.media";

        private static readonly string UniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

        public static CastMessage Play(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DefaultMediaUrn,
            PayloadUtf8 = new PlayRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Pause(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            
            Namespace = DefaultMediaUrn,
            PayloadUtf8 = new PauseRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Load(string destinationId, string payload) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DefaultMediaUrn,
            PayloadUtf8 = payload
        };

        public static CastMessage Seek(string destinationId, long mediaSessionId, double seconds)
            => new CastMessage(destinationId, UniqueSourceID)
            {
                Namespace =  DefaultMediaUrn,
                PayloadUtf8 = new SeekRequest(mediaSessionId, seconds).ToJson()
            };

        public static CastMessage MediaStatus(string destinationId) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = DefaultMediaUrn,
            PayloadUtf8 = new MediaStatusRequest().ToJson()
        };
        
    }

}
