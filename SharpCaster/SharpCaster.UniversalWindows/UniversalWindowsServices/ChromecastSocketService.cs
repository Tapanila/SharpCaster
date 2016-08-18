using SharpCaster.Extensions;
using SharpCaster.Interfaces;
using SharpCaster.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using SharpCaster.Channels;

namespace SharpCaster.Services
{
    public class ChromecastSocketService : IChromecastSocketService
    {
        private StreamSocket _socket;

        public async Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel, Action<Stream, bool> packetReader)
        {
            _socket = new StreamSocket().ConfigureForChromecast();
            await _socket.ConnectAsync(new HostName(host), port, SocketProtectionLevel.Tls10);

            connectionChannel.OpenConnection();
            heartbeatChannel.StartHeartbeat();

            await Task.Run(() =>
            {
                while (true)
                {
                    packetReader(_socket.InputStream.AsStreamForRead(), false);
                }
            });
        }

        public async Task Write(byte[] bytes)
        {
            var buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await _socket.OutputStream.WriteAsync(buffer);
        }
    }
}
