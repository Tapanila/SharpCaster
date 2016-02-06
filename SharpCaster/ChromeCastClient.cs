using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using SharpCaster.Helpers;
using SharpCaster.Models.ChromecastStatus;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public ChromecastChannel ConnectionChannel { get; set; }
        public ChromecastChannel HeartbeatChannel { get; set; }
        public ChromecastChannel ReceiverChannel { get; set; }
        public ChromecastChannel MediaChannel { get; set; }
        
        public List<ChromecastChannel> Channels { get; set; }

        private const string ChromecastPort = "8009";
        private string ChromecastApplicationId;
        private string currentApplicationSessionId = "";
        private string currentApplicationTransportId = "";
        private StreamSocket socket;
        private bool connected = false;

        public event EventHandler Connected;
        public event EventHandler<ChromecastApplication> ApplicationStarted;

        public ChromeCastClient()
        {
            Channels = new List<ChromecastChannel>();
            ConnectionChannel = CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
            HeartbeatChannel = CreateChannel(MessageFactory.DialConstants.DialHeartbeatUrn);
            ReceiverChannel = CreateChannel(MessageFactory.DialConstants.DialReceiverUrn);
            MediaChannel = CreateChannel(MessageFactory.DialConstants.DialMediaUrn);

            MediaChannel.MessageReceived += MediaChannel_MessageReceived;
            ReceiverChannel.MessageReceived += ReceiverChannel_MessageReceived;
            HeartbeatChannel.MessageReceived += HeartbeatChannel_MessageReceived;
        }

        private void HeartbeatChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            if (connected == false && e.Message.GetJsonType() == "PONG")
            {
                connected = true;
                Connected.Invoke(this, EventArgs.Empty);
            }
        }

        public async void SendMedia(string mediaUrl, object customData)
        {
            var mediaObject = new Media(mediaUrl, "application/vnd.ms-sstr+xml", null, "BUFFERED", 0D, customData);
            var req = new LoadRequest(currentApplicationSessionId, mediaObject, true, 0.0, customData);

            var reqJson = req.ToJson();
            await MediaChannel.Write(MessageFactory.Load(currentApplicationTransportId, reqJson));
        }

        private void MediaChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
        }

        private async void ReceiverChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<ChromecastStatusResponse>(json);
            var startedApplication = response?.status?.applications?.FirstOrDefault(x => x.appId == ChromecastApplicationId);
            if (startedApplication == null) return;
            if (currentApplicationSessionId != "") return;
            currentApplicationSessionId = startedApplication.sessionId;
            currentApplicationTransportId = startedApplication.transportId;
            await Write(CastHelper.ToProto(MessageFactory.Connect(startedApplication.transportId)));
            ApplicationStarted.Invoke(this, startedApplication);
        }



        public async void ConnectChromecast(Uri uri)
        {
            try
            {
                socket = new StreamSocket();

                //Handle MessageReceived
                socket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                socket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
                socket.Control.OutboundBufferSizeInBytes = 2048;

                socket.Control.KeepAlive = true;
                socket.Control.QualityOfService = SocketQualityOfService.LowLatency;


                await socket.ConnectAsync(new HostName(uri.Host), ChromecastPort, SocketProtectionLevel.Tls10);

                OpenConnection();
                StartHeartbeat();

                await Task.Run(() =>
                {
                    while (true)
                    {
                        ReadPacket(socket.InputStream.AsStreamForRead());
                    }
                });
            }
            catch (Exception ex)
            {
                var i = 0;
            }
        }

        public void LaunchApplication(string applicationId)
        {
            ChromecastApplicationId = applicationId;
            Write(CastHelper.ToProto(MessageFactory.Launch(applicationId)));
        }

        private void ReadPacket(Stream stream)
        {

            try
            {
                var entireMessage = ParseData(stream);
                try
                {
                    var entireMessageArray = entireMessage.ToArray();
                    var castMessage = CastHelper.ToCastMessage(entireMessageArray);
                    if (castMessage == null) return;
                    Debug.WriteLine("Received: " + castMessage.GetJsonType());
                    if (string.IsNullOrEmpty(castMessage.Namespace)) return;
                    ReceivedMessage(castMessage);
                }
                catch (Exception ex)
                {
                    // Log these bytes?
                    Debug.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                // Log these bytes?
                Debug.WriteLine(ex);
            }

        }

        private void ReceivedMessage(CastMessage castMessage)
        {
            foreach (var channel in Channels.Where(i => i.Namespace == castMessage.Namespace))
            {
                channel.OnMessageReceived(new ChromecastSSLClientDataReceivedArgs(castMessage));
            }
        }


        private async void OpenConnection()
        {
            Debug.WriteLine("Connect");
            await Write(CastHelper.ToProto(MessageFactory.Connect()));
        }

        private ChromecastChannel CreateChannel(string @namespace)
        {
            var channel = new ChromecastChannel(this, @namespace);
            Channels.Add(channel);
            return channel;
        }

        private void StartHeartbeat()
        {
            var channel = Channels.FirstOrDefault(i => i.Namespace == MessageFactory.DialConstants.DialHeartbeatUrn);

#pragma warning disable 4014
            Task.Run(async () =>
            {
                while (true)
                {
                    channel.Write(MessageFactory.Ping());
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
#pragma warning restore 4014
        }

        public async Task Write(byte[] bytes)
        {
            IBuffer buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await socket.OutputStream.WriteAsync(buffer);
        }

        private static IEnumerable<byte> ParseData(Stream _stream)
        {
            byte[] buffer = new byte[2048];

            var header = new List<byte>();
            var data = new List<byte>();


            var escape = true;

            while (escape)
            {
                // tricky byte order for messages
                var bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 1)
                {
                    // Incoming series of header /data
                    header.Add(buffer[0]);

                    bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 3)
                    {
                        header.Add(buffer[0]);
                        header.Add(buffer[1]);
                        header.Add(buffer[2]);

                        bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 1)
                        {
                            header.Add(buffer[0]);
                            bytesRead = _stream.Read(buffer, 0, buffer.Length);
                            data.AddRange(buffer.Take(bytesRead));
                            escape = false;
                        }
                        else
                        {
                            escape = false;
                        }
                    }
                    else
                    {
                        escape = false;
                    }
                }
                else
                {
                    escape = false;
                }
            }

            var entireMessage = new List<byte>();
            entireMessage.AddRange(header);
            entireMessage.AddRange(data);

            return entireMessage;
        }
    }
}
