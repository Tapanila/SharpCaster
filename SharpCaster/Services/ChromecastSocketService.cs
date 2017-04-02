using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Interfaces;
using Sockets.Plugin;

namespace SharpCaster.Services
{
    public class ChromecastSocketService : IChromecastSocketService
    {
        private TcpSocketClient _client;

        public async Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel, Action<Stream, bool> packetReader)
        {
            if (_client == null) _client = new TcpSocketClient();
            await _client.ConnectAsync(host, int.Parse(port), true, default(CancellationToken), true);


            connectionChannel.OpenConnection();
            heartbeatChannel.StartHeartbeat();

            await Task.Run(async () =>
            {
                while (true)
                {
                    var sizeBuffer = new byte[4];
                    // First message should contain the size of message
                    await _client.ReadStream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);
                    // The message is little-endian (that is, little end first),
                    // reverse the byte array.
                    Array.Reverse(sizeBuffer);
                    //Retrieve the size of message
                    var messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    var messageBuffer = new byte[messageSize];
                    await _client.ReadStream.ReadAsync(messageBuffer, 0, messageBuffer.Length);
                    var answer = new MemoryStream(messageBuffer.Length);
                    answer.Write(messageBuffer, 0, messageBuffer.Length);
                    answer.Position = 0;
                    packetReader(answer, true);
                }
            });
        }

        public async Task Write(byte[] bytes)
        {
            await _client.WriteStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}