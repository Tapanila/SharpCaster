using System;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models;

namespace SharpCaster.MediaControllers
{
    public static class PlexMediaMessageFactory
    {
        public const string PlexMediaUrn = "urn:x-cast:plex";

        private static readonly string UniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

        public static CastMessage Play(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {
            Namespace = PlexMediaUrn,
            PayloadUtf8 = new PlayRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Pause(string destinationId, long mediaSessionId) => new CastMessage(destinationId, UniqueSourceID)
        {

            Namespace = PlexMediaUrn,
            PayloadUtf8 = new PauseRequest(mediaSessionId).ToJson()
        };

        public static CastMessage Seek(string destinationId, long mediaSessionId, double seconds)
            => new CastMessage(destinationId, UniqueSourceID)
            {
                Namespace = PlexMediaUrn,
                PayloadUtf8 = new SeekRequest(mediaSessionId, seconds).ToJson()
            };

    }

}
