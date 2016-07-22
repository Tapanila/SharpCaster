using SharpCaster.Extensions;
using SharpCaster.Interfaces;
using SharpCaster.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;

namespace SharpCaster.Services
{
    public class ChromecastSocketService : IChromecastSocketService
    {
        private StreamSocket _socket;

        public async Task Initialize(string host, string port, ChromecastChannel connectionChannel, ChromecastChannel heartbeatChannel, Action<Stream, bool> packetReader)
        {
            _socket = new StreamSocket().ConfigureForChromecast();
            await _socket.ConnectAsync(new HostName(host), port, SocketProtectionLevel.Tls10);

            OpenConnection(connectionChannel);
            StartHeartbeat(heartbeatChannel);

            await Task.Run(() =>
            {
                while (true)
                {
                    packetReader(_socket.InputStream.AsStreamForRead(), false);
                }
            });
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
            var buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await _socket.OutputStream.WriteAsync(buffer);
        }
    }
}
