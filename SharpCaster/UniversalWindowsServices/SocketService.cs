using SharpCaster.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SharpCaster.Services
{
    public class SocketService : ISocketService
    {
        private DatagramSocket _datagramSocket;

        public ISocketService Initialize()
        {
            _datagramSocket = new DatagramSocket();
            _datagramSocket.MessageReceived += _datagramSocket_MessageReceived;
            return this;
        }

        private void _datagramSocket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var reader = args.GetDataReader();
            var bytesRemaining = reader.UnconsumedBufferLength;
            var receivedString = reader.ReadString(bytesRemaining);

            MessageReceived?.Invoke(this, receivedString);
        }

        public event EventHandler<string> MessageReceived;

        public void Dispose()
        {
            _datagramSocket.MessageReceived -= _datagramSocket_MessageReceived;
            _datagramSocket.Dispose();
            _datagramSocket = null;
        }

        public async Task BindEndpointAsync(string localHostName, string localServiceName)
        {
            await _datagramSocket.BindEndpointAsync(new HostName(localHostName), localServiceName);
        }

        public void JoinMulticastGroup(string multicastIP)
        {
            _datagramSocket.JoinMulticastGroup(new HostName(multicastIP));
        }

        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            var http = new HttpClient { Timeout = timeout };
            return await http.GetStringAsync(uri);
        }

        public async Task Write(string request, string multicastPort, string multicastIP)
        {
            var writer = new DataWriter(await _datagramSocket.GetOutputStreamAsync(new HostName(multicastIP), multicastPort));
            writer.WriteString(request);
            await writer.StoreAsync();
        }
    }
}
