using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Channels
{
    public class HeartbeatChannel : ChromecastChannel
    {
        public HeartbeatChannel(ChromeCastClient client) : 
            base(client, "urn:x-cast:com.google.cast.tp.heartbeat")
        {
            
            MessageReceived += HeartbeatChannel_MessageReceived;
        }

        private async void HeartbeatChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            Debug.WriteLine(e.Message.GetJsonType());
            if (Client.Connected || e.Message.GetJsonType() != "PONG") return;
            //Wait 100 milliseconds before sending GET_STATUS because chromecast was sending CLOSE back without a wait
            await Task.Delay(100);
            Client.ReceiverChannel.GetChromecastStatus();
            //Wait 100 milliseconds to make sure that the status of Chromecast device is received before notifying we have connected to it
            await Task.Delay(100);
            Client.Connected = true;
        }

        public void StartHeartbeat()
        {
            Task.Run(async () =>
            {
                //TODO: This should stop if we receive an disconnect or close from socket
                while (true)
                {
                    await Write(MessageFactory.Ping);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }
    }
}
