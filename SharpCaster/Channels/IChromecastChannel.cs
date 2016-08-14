using System;
using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Channels
{
    public interface IChromecastChannel
    {
        string Namespace { get; }
        event EventHandler<ChromecastSSLClientDataReceivedArgs> MessageReceived;
        Task Write(CastMessage message, bool includeNameSpace = true);
        void OnMessageReceived(ChromecastSSLClientDataReceivedArgs e);
    }
}