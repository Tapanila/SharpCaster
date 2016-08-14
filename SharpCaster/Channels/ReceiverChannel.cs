using System;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;

namespace SharpCaster.Channels
{
    public class ReceiverChannel : ChromecastChannel 
    {
        public ReceiverChannel(ChromeCastClient client) :
            base(client, MessageFactory.DialConstants.DialReceiverUrn)
        {
            MessageReceived += ReceiverChannel_MessageReceived;
        }

        private async void ReceiverChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<ChromecastStatusResponse>(json);
            if (response.ChromecastStatus == null) return;
            Client.ChromecastStatus = response.ChromecastStatus;
            Client.Volume = response.ChromecastStatus.Volume;
            await Client.ConnectToApplication(Client.ChromecastApplicationId);
        }

        public async void GetChromecastStatus()
        {
            await Write(MessageFactory.Status());
        }
        
    }
}
