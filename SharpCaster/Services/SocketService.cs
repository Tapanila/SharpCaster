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

        public async Task BindEndpointAsync(HostName localHostName, string localServiceName)
        {
            await _datagramSocket.BindEndpointAsync(localHostName, localServiceName);
        }

        public void JoinMulticastGroup(HostName multicastIP)
        {
            _datagramSocket.JoinMulticastGroup(multicastIP);

        }

        public async Task<DataWriter> GetOutputWriterAsync(HostName multicastIP, string multicastPort)
        {
            return new DataWriter(await _datagramSocket.GetOutputStreamAsync(multicastIP, multicastPort));
        }

        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            var http = new HttpClient { Timeout = timeout };
            return await http.GetStringAsync(uri);
        }
    }
}
