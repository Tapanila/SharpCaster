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
        private static readonly object LockObject = new object();
        private TcpSocketClient _client;

        public async Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel, Action<Stream, bool, CancellationToken> packetReader, CancellationToken cancellationToken)
        {
            if (_client == null) _client = new TcpSocketClient();
            await _client.ConnectAsync(host, int.Parse(port), true, cancellationToken, true);

            await connectionChannel.OpenConnection();
            heartbeatChannel.StartHeartbeat();

            await Task.Run(async () =>
            {
                while (true)
                {
                    var sizeBuffer = new byte[4];
                    // First message should contain the size of message
                    await _client.ReadStream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length, cancellationToken);
                    // The message is little-endian (that is, little end first),
                    // reverse the byte array.
                    Array.Reverse(sizeBuffer);
                    //Retrieve the size of message
                    var messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    var messageBuffer = new byte[messageSize];
                    await _client.ReadStream.ReadAsync(messageBuffer, 0, messageBuffer.Length, cancellationToken);
                    var answer = new MemoryStream(messageBuffer.Length);
                    await answer.WriteAsync(messageBuffer, 0, messageBuffer.Length, cancellationToken);
                    answer.Position = 0;
                    packetReader(answer, true, cancellationToken);
                }
            }, cancellationToken);
        }

        #pragma warning disable 1998
        public async Task Write(byte[] bytes, CancellationToken cancellationToken)
        #pragma warning restore 1998
        {
            if (_client == null) return;
            
            lock (LockObject)
            {
                if (cancellationToken.IsCancellationRequested) return;
                try
                {
                    _client.WriteStream.Write(bytes, 0, bytes.Length);
                }
                catch (IOException)
                {
                    //We have been disconnected from chromecast
                    //TODO: Raise an disconnected event
                }
            }
        }

        public async Task Disconnect()
        {
            await _client.DisconnectAsync();
            _client = null;
        }
    }
}