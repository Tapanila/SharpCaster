using SharpCaster;
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
    public class ChromecastSocketService : BaseChromecastSocketService, IChromecastSocketService
    {
        private TcpClient _client;
        private SslStream _stream;

        public async Task Initialize(string host, string port, ChromecastChannel connectionChannel)
        {
            if (_client == null) _client = new TcpClient();
            _client.ReceiveBufferSize = 2048;
            _client.SendBufferSize = 2048;
            await _client.ConnectAsync(host, int.Parse(port));
            _stream = new SslStream(_client.GetStream(), true, ValidateServerCertificate, null);
            
            _stream.AuthenticateAsClient("client");
            
        
            OpenConnection(connectionChannel);
            
            await Task.Run(() =>
            {
                while (true)
                {
                    var sizeBuffer = new byte[4];
                    byte[] messageBuffer = { };
                    // First message should contain the size of message
                    _stream.Read(sizeBuffer, 0, sizeBuffer.Length);
                    // The message is little-endian (that is, little end first),
                    // reverse the byte array.
                    Array.Reverse(sizeBuffer);
                    //Retrieve the size of message
                    var messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    messageBuffer = new byte[messageSize];
                    _stream.Read(messageBuffer, 0, messageBuffer.Length);
                    var answer = new MemoryStream(messageBuffer.Length);
                    answer.Write(messageBuffer,0,messageBuffer.Length);
                    answer.Position = 0;
                    ReadPacket(answer,true);
                }
            });
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private async void OpenConnection(ChromecastChannel connectionChannel)
        {
            await connectionChannel.Write(MessageFactory.Connect());
        }

        public Task Write(byte[] bytes)
        {
            _stream.Write(bytes, 0, bytes.Length);
            return Task.Delay(0);
        }
    }
}
