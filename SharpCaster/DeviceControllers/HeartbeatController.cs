using SharpCaster.Interfaces;
using SharpCaster.Models;
using System;
using System.Threading.Tasks;

namespace SharpCaster.DeviceControllers
{

    public class HeartbeatController : IHeartbeatController
    {
        private ChromeCastClient _chromecastClient;
        private ChromecastChannel _heartbeatChannel;

        public HeartbeatController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;
            _heartbeatChannel = _chromecastClient.CreateChannel(MessageFactory.DialConstants.DialHeartbeatUrn);
            
            _heartbeatChannel.MessageReceived += HeartbeatChannel_MessageReceived;
        }

        public void StartHeartbeat()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await _heartbeatChannel.Write(MessageFactory.Ping);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }

        private async void HeartbeatChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            if (_chromecastClient.IsConnected || e.Message.GetJsonType() != "PONG") return;
            //Wait 100 milliseconds before sending GET_STATUS because chromecast was sending CLOSE back without a wait
            await Task.Delay(100);
            _chromecastClient.ReceiverController.GetChromecastStatus();
            //Wait 100 milliseconds to make sure that the status of Chromecast device is received before notifying we have connected to it
            await Task.Delay(100);
            _chromecastClient.IsConnected = true;
            
        }
    }
}
