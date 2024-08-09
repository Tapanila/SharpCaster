using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Multizone;
using Sharpcaster.Messages.Spotify;
using Sharpcaster.Models.MultiZone;
using Sharpcaster.Models.Spotify;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    public class SpotifyChannel : ChromecastChannel
    { 
        /// <summary>
        /// Raised when device has been updated
        /// </summary>
        public event EventHandler<SpotifyStatus> SpotifyStatusUpdated;

        public SpotifyStatus SpotifyStatus{ get; set; }

        public SpotifyChannel(ILogger<SpotifyChannel> logger = null) : base("urn:x-cast:com.spotify.chromecast.secure.v1", logger, false)
        {

        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public override Task OnMessageReceivedAsync(IMessage message)
        {
            switch (message)
            {
                case GetInfoResponseMessage getInfoResponseMessage:
                    SpotifyStatus = getInfoResponseMessage.Status;
                    SpotifyStatusUpdated?.Invoke(this, getInfoResponseMessage.Status);
                    break;
                default:
                    break;
            }

            return base.OnMessageReceivedAsync(message);
        }

        /// <summary>
        /// Raises the StatusChanged event
        /// </summary>
        protected virtual void OnStatusChanged()
        {
        }

        public async Task GetSpotifyInfo()
        {
            await SendAsync(new GetInfoMessage
            {
                deviceId = Client.FriendlyName,
                remoteName = SpotifyDeviceId,
                deviceAPI_isGroup = false
            });
        }

        public string SpotifyDeviceId { get
            {
                var friendlyName = Client.FriendlyName;
                return ComputeMd5Hash(friendlyName);
            }
        }

        public static string ComputeMd5Hash(string input)
        {
            // Create an instance of the MD5 service provider
            using (MD5 md5 = MD5.Create())
            {
                // Compute the hash as a byte array
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert the byte array to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
