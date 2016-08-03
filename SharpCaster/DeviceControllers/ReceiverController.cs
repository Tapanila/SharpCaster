using Newtonsoft.Json;
using SharpCaster.Interfaces;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using System;
using System.Threading.Tasks;

namespace SharpCaster.DeviceControllers
{
    public class ReceiverController : IReceiverController
    {
        private ChromeCastClient _chromecastClient;
        private ChromecastChannel _receiverChannel;

        public ReceiverController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;
            _receiverChannel = _chromecastClient.CreateChannel(MessageFactory.DialConstants.DialHeartbeatUrn);

            _receiverChannel.MessageReceived += ReceiverChannel_MessageReceived;
        }

        public async void GetChromecastStatus()
        {
            await _receiverChannel.Write(MessageFactory.Status());
        }

        public async Task SetVolume(float level)
        {
            if (level < 0 || level > 1.0f)
            {
                throw new ArgumentException("level must be between 0.0f and 1.0f", nameof(level));
            }
            await _chromecastClient.ChromecastSocketService.Write(MessageFactory.Volume(level).ToProto());
        }

        public async Task SetMute(bool muted)
        {
            await _chromecastClient.ChromecastSocketService.Write(MessageFactory.Volume(muted).ToProto());
        }

        public async Task StopApplication()
        {
            await _chromecastClient.ChromecastSocketService.Write(MessageFactory.Stop(_chromecastClient.CurrentApplicationSessionId).ToProto());
        }

        private async void ReceiverChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<ChromecastStatusResponse>(json);
            if (response.ChromecastStatus == null) return;

            var status = response.ChromecastStatus;
            _chromecastClient.UpdateStatus(status);
            _chromecastClient.UpdateVolume(status.Volume);

            //TODO I'm not a fan of calling ConnectToApplication here, the ChromeClient should orchestrate this.
            await _chromecastClient.ConnectionController.ConnectToApplication(_chromecastClient.ChromecastApplicationId);
        }
    }
}
