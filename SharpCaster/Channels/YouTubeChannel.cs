using System;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.CustomTypes;

namespace SharpCaster.Channels
{
    public class YouTubeChannel : ChromecastChannel
    {
        public event EventHandler<string> ScreenIdChanged;
        public YouTubeChannel(ChromeCastClient client) : base(client, MessageFactory.DialConstants.YouTubeUrn)
        {
            MessageReceived += YouTubeChannel_MessageReceived;
        }

        private void YouTubeChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<YouTubeSessionStatusResponse>(json);
            ScreenIdChanged?.Invoke(this, response.Data.ScreenId);
        }
    }
}
