using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Messages.Spotify;
using Sharpcaster.Models.Spotify;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    public class SpotifyChannel : ChromecastChannel
    {
        public event EventHandler<SpotifyStatus>? SpotifyStatusUpdated;
        public SpotifyStatus? SpotifyStatus { get; set; }
        public event EventHandler<AddUserResponseMessagePayload>? AddUserResponseReceived;

        public SpotifyChannel(ILogger? logger = null) : base("urn:x-cast:com.spotify.chromecast.secure.v1", logger, false)
        {
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="messagePayload">message payload to process</param>
        /// <param name="type">message type</param>
        public override void OnMessageReceived(string messagePayload, string type)
        {
            switch (type)
            {
                case "getInfoResponse":
                    var getInfoResponseMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.GetInfoResponseMessage);
                    if (getInfoResponseMessage?.Payload != null)
                    {
                        SpotifyStatus = getInfoResponseMessage.Payload;
                        SafeInvokeEvent(SpotifyStatusUpdated, this, getInfoResponseMessage.Payload);
                    }
                    break;
                case "addUserResponse":
                    var addUserResponseMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.AddUserResponseMessage);
                    if (addUserResponseMessage?.Payload != null)
                    {
                        SafeInvokeEvent(AddUserResponseReceived, this, addUserResponseMessage.Payload);
                    }
                    break;
            }
        }

        /// <summary>
        /// Raises the StatusChanged event
        /// </summary>
        protected virtual void OnStatusChanged()
        {
        }

        public async Task GetSpotifyInfo()
        {
            var spotifyInfoMessage = new GetInfoMessage
            {
                Payload = new GetInfoMessagePayload
                {
                    DeviceId = SpotifyDeviceId,
                    RemoteName = Client.FriendlyName,
                    DeviceAPI_isGroup = false
                }
            };
            await SendAsync(JsonSerializer.Serialize(spotifyInfoMessage, SharpcasteSerializationContext.Default.GetInfoMessage), Client.ChromecastStatus.Application.TransportId).ConfigureAwait(false);
        }

        public async Task AddUser(string accessToken)
        {
            var addUserMessage = new AddUserMessage
            {
                Payload = new AddUserMessagePayload
                {
                    Blob = accessToken,
                    TokenType = "accesstoken"
                }
            };
            await SendAsync(addUserMessage.RequestId, JsonSerializer.Serialize(addUserMessage, SharpcasteSerializationContext.Default.AddUserMessage), Client.ChromecastStatus.Application.TransportId).ConfigureAwait(false);
        }

        public string SpotifyDeviceId
        {
            get
            {
                var friendlyName = Client.FriendlyName;
                return ComputeMd5Hash(friendlyName);
            }
        }

        public static string ComputeMd5Hash(string input)
        {
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
#if NETSTANDARD2_0
            // Create an instance of the MD5 service provider
            using MD5 md5 = MD5.Create();
            // Compute the hash as a byte array
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
#else
            byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
#endif
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
            // Convert the byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }
    }
}
