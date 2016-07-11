using SharpCaster.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpCaster.Services
{
    public class SocketService : ISocketService
    {
        private Socket _datagramSocket;
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private readonly byte[] _buffer = new byte[9000];

        public ISocketService Initialize()
        {
            _datagramSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _datagramSocket.ReceiveTimeout = 1000;
            _datagramSocket.SendTimeout = 1000;
            _datagramSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            _datagramSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            _datagramSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, (int)ushort.MaxValue);
           
            return this;
        }

     
        public event EventHandler<string> MessageReceived;

        private void OnReceive(IAsyncResult ar)
        {
            var length = _datagramSocket.EndReceive(ar);
            using (var stream = new MemoryStream(_buffer, 0, length))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    MessageReceived?.Invoke(this, reader.ReadToEnd());
                }
            }
        }

        public void Dispose()
        {
            _datagramSocket.Dispose();
            _datagramSocket = null;
        }

        public Task BindEndpointAsync(string localHostName, string localServiceName)
        {
            //_datagramSocket.Bind(new IPEndPoint(long.Parse(localHostName), 1900));
            return null;
        }

        public void JoinMulticastGroup(string multicastIP)
        {
            _datagramSocket.Bind(new IPEndPoint(long.Parse(multicastIP), 1900));
        }

        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            var http = new HttpClient { Timeout = timeout };
            return await http.GetStringAsync(uri);
        }

        public async Task Write(string request, string multicastPort, string multicastIP)
        {
            var bytes = Encoding.UTF8.GetBytes(request);
            _datagramSocket.SendTo(bytes, new IPEndPoint(IPAddress.Parse(multicastIP), int.Parse(multicastPort)));
            _datagramSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
        }
    }
}
