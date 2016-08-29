using SharpCaster.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Java.Net;


namespace SharpCaster.Services
{
    public class SocketService : ISocketService
    {
        
        private MulticastSocket _datagramSocket;
        private readonly byte[] _buffer = new byte[1000];
        private InetAddress _group;
        WifiManager.MulticastLock _multicastLock;


        public ISocketService Initialize()
        {
            var manager = Application.Context.GetSystemService(Context.WifiService) as WifiManager;
            _multicastLock = manager.CreateMulticastLock("SharpCaster");
            _multicastLock.Acquire();
            _datagramSocket = new MulticastSocket(1900);
            return this;
        }

     
        public event EventHandler<string> MessageReceived;

        public void Dispose()
        {
            _multicastLock.Release();
            _datagramSocket.Dispose();
            _datagramSocket = null;
        }

        public Task BindEndpointAsync(string localHostName, string localServiceName)
        {
            return Task.FromResult(0);
        }

        public void JoinMulticastGroup(string multicastIP)
        {
            _group = InetAddress.GetByName(multicastIP);
            _datagramSocket.JoinGroup(_group);
            Task.Run(() =>
            {
                var respone = new DatagramPacket(_buffer, _buffer.Length);
                while (_datagramSocket != null)
                {
                    _datagramSocket.Receive(respone);
                    Debug.WriteLine(respone.Length + " bytes from " + respone.Address);
                    using (var stream = new MemoryStream(respone.GetData(), 0, respone.Length))
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            MessageReceived?.Invoke(this, reader.ReadToEnd());
                        }
                    }
                    respone.Length = _buffer.Length;
                }
            });
        }



        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            var http = new HttpClient { Timeout = timeout };
            return await http.GetStringAsync(uri);
        }

        public async Task Write(string request, string multicastPort, string multicastIP)
        {
            await Task.Run(() =>
            {
                var requestBytes = Encoding.UTF8.GetBytes(request);
                var packet = new DatagramPacket(requestBytes, requestBytes.Length, _group, int.Parse(multicastPort));
                _datagramSocket.Send(packet);
            });
        }
    }
}
