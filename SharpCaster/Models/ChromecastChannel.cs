using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SharpCaster.Models
{
    public class ChromecastChannel
    {
        private ChromeCastClient Client { get; set; }
        public string Namespace { get; set; }

        public event EventHandler<ChromecastSSLClientDataReceivedArgs> MessageReceived;

        public ChromecastChannel(ChromeCastClient client, string @ns)
        {
            Namespace = ns;
            Client = client;
        }

        public async Task Write(CastMessage message)
        {
            Debug.WriteLine("Sending: " + message.GetJsonType());
            message.Namespace = Namespace;

            var bytes = message.ToProto();
            await Client.Write(bytes);
        }

        public void OnMessageReceived(ChromecastSSLClientDataReceivedArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}
