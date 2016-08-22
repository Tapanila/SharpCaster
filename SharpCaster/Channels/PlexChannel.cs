using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.CustomTypes;

namespace SharpCaster.Channels
{
    public class PlexChannel : ChromecastChannel
    {
        public PlexChannel(ChromeCastClient client) : base(client, MessageFactory.DialConstants.PlexUrn)
        {
            MessageReceived += PlexChannel_MessageReceived;
        }

        public async Task Play()
        {
            //Plex uses the same messages as the Google Media messages but on its own namespace
            var castMessage = MessageFactory.Play(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId);

            await Write(castMessage);
        }

        public async Task Pause()
        {
            //Plex uses the same messages as the Google Media messages but on its own namespace
            var castMessage = MessageFactory.Pause(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId);
            
            await Write(castMessage);
        }

        public async Task Seek(double seconds)
        {
            //Plex uses the same messages as the Google Media messages but on its own namespace
            var castMessage = MessageFactory.Seek(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId, seconds);

            await Write(castMessage);
        }

        public async Task Stop()
        {
            await Write(MessageFactory.StopMedia(Client.CurrentMediaSessionId));
        }

        //        this.customMessageListeners[d.PREVIOUS] = this.onPrevious,
        //        this.customMessageListeners[d.NEXT] = this.onNext,
        //        this.customMessageListeners[d.SKIPTO] = this.onSkipTo,
        //        this.customMessageListeners[d.SHOWDETAILS] = this.onShowDetails,
        //        this.customMessageListeners[d.REFRESHPLAYQUEUE] = this.onRefreshPlayQueue,
        //        this.customMessageListeners[d.SETQUALITY] = this.onSetQuality,
        //        this.customMessageListeners[d.SETSTREAM
        
        private static CastMessage ToCastMessage(object messageObject)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var payload = JsonConvert.SerializeObject(messageObject, settings);
            var message = new CastMessage()
            {
                PayloadUtf8 = payload
            };
            return message;
        }

        private void PlexChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<YouTubeSessionStatusResponse>(json);
        }
    }
}
