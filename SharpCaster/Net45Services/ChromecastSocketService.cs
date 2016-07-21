using SharpCaster.Interfaces;
using SharpCaster.Models;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SharpCaster.Services
{
    public class ChromecastSocketService : IChromecastSocketService
    {
        private TcpClient _client;
        private SslStream _stream;

        public async Task Initialize(string host, string port, ChromecastChannel connectionChannel, ChromecastChannel heartbeatChannel, Action<Stream> packetReader)
        {
            if (_client == null) _client = new TcpClient();
            _client.ReceiveBufferSize = 2048;
            _client.SendBufferSize = 2048;
            await _client.ConnectAsync(host, int.Parse(port));
            _stream = new SslStream(_client.GetStream(), true, ValidateServerCertificate, null);
            _stream.AuthenticateAsClient("client");
            
        
            OpenConnection(connectionChannel);
            StartHeartbeat(heartbeatChannel);
            
            await Task.Run(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    _stream.Read(buffer, 0, buffer.Length);
                    var stream = new MemoryStream(buffer);
                    packetReader(stream);
                }
            });
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void StartHeartbeat(ChromecastChannel hearbeatChannel)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await hearbeatChannel.Write(MessageFactory.Ping);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }

        private async void OpenConnection(ChromecastChannel connectionChannel)
        {
            await connectionChannel.Write(MessageFactory.Connect());
        }

        public async Task Write(byte[] bytes)
        {
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
