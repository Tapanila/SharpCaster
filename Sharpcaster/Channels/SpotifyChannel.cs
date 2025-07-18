﻿using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Messages.Spotify;
using Sharpcaster.Models.Spotify;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    public class SpotifyChannel : ChromecastChannel
    {
        public event EventHandler<SpotifyStatus> SpotifyStatusUpdated;
        public SpotifyStatus SpotifyStatus { get; set; }
        public event EventHandler<AddUserResponseMessagePayload> AddUserResponseReceived;

        public SpotifyChannel(ILogger<SpotifyChannel> logger = null) : base("urn:x-cast:com.spotify.chromecast.secure.v1", logger, false)
        {
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            switch (type)
            {
                case "getInfoResponse":
                    var getInfoResponseMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.GetInfoResponseMessage);
                    SpotifyStatus = getInfoResponseMessage.Payload;
                    SpotifyStatusUpdated?.Invoke(this, getInfoResponseMessage.Payload);
                    break;
                case "addUserResponse":
                    var addUserResponseMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.AddUserResponseMessage);
                    AddUserResponseReceived?.Invoke(this, addUserResponseMessage.Payload);
                    break;
            }

            return base.OnMessageReceivedAsync(messagePayload, type);
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
            await SendAsync(JsonSerializer.Serialize(spotifyInfoMessage, SharpcasteSerializationContext.Default.GetInfoMessage), Client.GetChromecastStatus().Application.TransportId);
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
            await SendAsync(addUserMessage.RequestId, JsonSerializer.Serialize(addUserMessage, SharpcasteSerializationContext.Default.AddUserMessage), Client.GetChromecastStatus().Application.TransportId);
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
