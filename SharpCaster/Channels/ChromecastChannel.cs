using System;
using System.Threading.Tasks;
using Extensions.Api.CastChannel;
using Google.Protobuf;
using SharpCaster.Models;

namespace SharpCaster.Channels
{
    public abstract class ChromecastChannel : IChromecastChannel
    {
        protected ChromeCastClient Client { get; }
        public string Namespace { get;}

        public event EventHandler<ChromecastSSLClientDataReceivedArgs> MessageReceived;

        protected ChromecastChannel(ChromeCastClient client, string ns)
        {
            Namespace = ns;
            Client = client;
        }
        
        public async Task Write(CastMessage message, bool includeNameSpace = true)
        {
            if (includeNameSpace)
            {
                message.Namespace = Namespace;
            }
            var bytes = message.ToProto();
            await Client.ChromecastSocketService.Write(bytes, Client.CancellationTokenSource.Token);
        }

        public void OnMessageReceived(ChromecastSSLClientDataReceivedArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}