using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Extensions;

namespace SharpCaster.Controllers
{
    public class PlexController : BaseMediaController
    {
        public PlexController(ChromeCastClient client) : base(client, "9AC194DC")
        {
        }

        public override async Task Play()
        {
            await Client.Channels.GetPlexChannel().Play();
        }

        public override async Task Pause()
        {
            await Client.Channels.GetPlexChannel().Pause();
        }

        public override async Task Seek(double seconds)
        {
            await Client.Channels.GetPlexChannel().Seek(seconds);
        }

        public override async Task Stop()
        {
            await Client.Channels.GetPlexChannel().Stop();
        }

        public override async Task Next()
        {
            await Client.Channels.GetPlexChannel().Next();
        }

        public override async Task Previous()
        {
            await Client.Channels.GetPlexChannel().Previous();
        }

        //public async Task Load()
        //{
        //    throw new NotImplementedException();

            //await Client.MediaChannel

            // Load uses the LOAD request on the default MediaChannel but it uses the customData and contentId fields
            // The customData sets player preferences and the server where the app should get its data from
            // This data needs to be obtained using Plex' REST api

            //{  
            //   "type":"LOAD",
            //   "requestId":165960142,
            //   "sessionId":"3D0CDA8B-83D9-4CBE-BACE-A97A21E9F04F",
            //   "media":{  
            //      "contentId":"/library/metadata/2769",
            //      "streamType":"BUFFERED",
            //      "contentType":"music",
            //      "customData":{  
            //         "offset":0,
            //         "directPlay":true,
            //         "directStream":true,
            //         "subtitleSize":100,
            //         "audioBoost":100,
            //         "server":{  
            //            "machineIdentifier":"40charLowercaseHex",
            //            "transcoderVideo":true,
            //            "transcoderVideoRemuxOnly":true,
            //            "transcoderAudio":true,
            //            "version":"1.0.3.2461",
            //            "myPlexSubscription":false,
            //            "isVerifiedHostname":true,
            //            "protocol":"https",
            //            "address":"ip-addr-with-dashes.32charLowercaseHex.plex.direct",
            //            "port":"32400",
            //            "accessToken":"transient-8charLowercaseHex-624f-4aea-83f5-12charLowercaseHex"
            //         },
            //         "user":{  
            //            "username":"userNameInPlex"
            //         },
            //         "containerKey":"/playQueues/56?own=1&window=200"
            //      }
            //   },
            //   "autoplay":true,
            //   "currentTime":0
            //}
        //}
    }

    public static class PlexControllerExtensions
    {
        public static async Task<PlexController> LaunchPlex(this ChromeCastClient client)
        {
            client.MakeSureChannelExist(new PlexChannel(client));
            var controller = new PlexController(client);
            await controller.LaunchApplication();
            return controller;
        }
    }
}
