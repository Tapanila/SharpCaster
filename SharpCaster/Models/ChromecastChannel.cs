using System;
using System.Diagnostics;
using System.Threading.Tasks;

using SharpCaster.Interfaces;

namespace SharpCaster.Models
{
    public class ChromecastChannel
    {
        private IChromecastSocketService SocketService { get; set; }
        public string Namespace { get; set; }

        public event EventHandler<ChromecastSSLClientDataReceivedArgs> MessageReceived;

        public ChromecastChannel(IChromecastSocketService socketService, string @ns)
        {
            Namespace = ns;
            SocketService = socketService;
        }

        public async Task Write(CastMessage message)
        {
            Debug.WriteLine("Sending: " + message.GetJsonType());
            message.Namespace = Namespace;

            var bytes = message.ToProto();
            await SocketService.Write(bytes);
        }

        public void OnMessageReceived(ChromecastSSLClientDataReceivedArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}
