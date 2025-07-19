using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Media;
using Sharpcaster.Messages.Queue;
using Sharpcaster.Messages.Receiver;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Media channel
    /// </summary>
    public class MediaChannel : ChromecastChannel, IMediaChannel
    {
        /// <summary>
        /// Raised when error is received
        /// </summary>
        public event EventHandler<ErrorMessage> ErrorHappened;
        public event EventHandler<LoadCancelledMessage> LoadCancelled;
        public event EventHandler<LoadFailedMessage> LoadFailed;    
        public event EventHandler<InvalidRequestMessage> InvalidRequest;
        public event EventHandler<MediaStatus> StatusChanged;

        public MediaStatus MediaStatus { get => mediaStatus; }
        private MediaStatus mediaStatus;
        /// <summary>
        /// Initializes a new instance of MediaChannel class
        /// </summary>
        public MediaChannel(ILogger<MediaChannel> logger = null) : base("media", logger)
        {
        }

        private async Task<MediaStatus> SendAsync(int messageRequestId, string messagePayload, ChromecastApplication application, bool DoNotReturnOnLoading = true)
        {
            try
            {
                var response = await SendAsync(messageRequestId, messagePayload, application.TransportId);
                var mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
                if (DoNotReturnOnLoading && mediaStatusMessage.Status?.FirstOrDefault()?.ExtendedStatus?.PlayerState == PlayerStateType.Loading)
                {
                    response = await Client.WaitResponseAsync(messageRequestId);
                    mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
                }
                return mediaStatusMessage.Status?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger?.LogError("Error sending message: {exceptionMessage}", ex.Message);
                mediaStatus = null;
                throw;
            }
        }

        private async Task<MediaStatus> SendAsync<T>(T message, JsonTypeInfo<T> serializationContext, bool DoNotReturnOnLoading = true) where T : MediaSessionMessage
        {
            var chromecastStatus = Client.GetChromecastStatus();
            message.MediaSessionId = MediaStatus?.MediaSessionId ?? throw new ArgumentNullException(nameof(message), "MediaSessionID");
            var messagePayload = JsonSerializer.Serialize(message, serializationContext);
            return await SendAsync(message.RequestId, messagePayload, chromecastStatus.Applications[0], DoNotReturnOnLoading);
        }

        /// <summary>
        /// Loads a media
        /// </summary>
        /// <param name="media">media to load</param>
        /// <param name="autoPlay">true to play the media directly, false otherwise</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus> LoadAsync(Media media, bool autoPlay = true)
        {
            var status = Client.GetChromecastStatus();
            var loadMessage = new LoadMessage() { SessionId = status.Application.SessionId, Media = media, AutoPlay = autoPlay };
            return await SendAsync(loadMessage.RequestId, JsonSerializer.Serialize(loadMessage, SharpcasteSerializationContext.Default.LoadMessage), status.Application);
        }

        public override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            switch (type)
            {
                case "LOAD_FAILED":
                    var loadFailedMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.LoadFailedMessage);
                    LoadFailed?.Invoke(this, loadFailedMessage);
                    return Task.CompletedTask;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.LoadCancelledMessage);
                    LoadCancelled?.Invoke(this, loadCancelledMessage);
                    return Task.CompletedTask;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.InvalidRequestMessage);
                    InvalidRequest?.Invoke(this, invalidRequestMessage);
                    return Task.CompletedTask;
                case "ERROR":
                    var errorMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.ErrorMessage);
                    ErrorHappened?.Invoke(this, errorMessage);
                    return Task.CompletedTask;
                case "MEDIA_STATUS":
                    var mediaStatusMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.MediaStatusMessage);
                    mediaStatus = mediaStatusMessage.Status.FirstOrDefault();
                    StatusChanged?.Invoke(this, MediaStatus);
                    return Task.CompletedTask;
                case "QUEUE_CHANGE":
                    return Task.CompletedTask;
            }
#if DEBUG
            Debugger.Break();
#endif
            return Task.CompletedTask;
        }

        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> PlayAsync()
        {
            return await SendAsync(new PlayMessage(), SharpcasteSerializationContext.Default.PlayMessage);
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> PauseAsync()
        {
            return await SendAsync(new PauseMessage(), SharpcasteSerializationContext.Default.PauseMessage);
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> StopAsync()
        {
            return await SendAsync(new StopMediaMessage(), SharpcasteSerializationContext.Default.StopMediaMessage);
        }

        /// <summary>
        /// Seeks to the specified time
        /// </summary>
        /// <param name="seconds">time in seconds</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus> SeekAsync(double seconds)
        {
            return await SendAsync(new SeekMessage() { CurrentTime = seconds }, SharpcasteSerializationContext.Default.SeekMessage);
        }

        public async Task<MediaStatus?> QueueLoadAsync(QueueItem[] items, long? currentTime = null, RepeatModeType repeatMode = RepeatModeType.OFF, long? startIndex = null)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            var queueLoadMessage = new QueueLoadMessage() { SessionId = chromecastStatus.Application.SessionId, Items = items, CurrentTime = currentTime, RepeatMode = repeatMode, StartIndex = startIndex };
            var response = await SendAsync(queueLoadMessage.RequestId, JsonSerializer.Serialize(queueLoadMessage, SharpcasteSerializationContext.Default.QueueLoadMessage), chromecastStatus.Application.TransportId);
            var mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
            if (mediaStatusMessage != null && mediaStatusMessage.Status != null)
            {
                return mediaStatusMessage.Status.FirstOrDefault();
            }
            return null;
        }

        public async Task<MediaStatus> QueueNextAsync()
        {
            //For some reason the request id is only returned with the first message that might be buffering, idle or playing and rest of the messages are without it
            await SendAsync(new QueueUpdateMessage { Jump = 1 }, SharpcasteSerializationContext.Default.QueueUpdateMessage, false);
            return await GetMediaStatusAsync();
        }

        public async Task<MediaStatus> QueuePrevAsync()
        {
            //For some reason the request id is only returned with the first message that might be buffering, idle or playing and rest of the messages are without it
            await SendAsync(new QueueUpdateMessage { Jump = -1}, SharpcasteSerializationContext.Default.QueueUpdateMessage, false);
            return await GetMediaStatusAsync();
        }

        public async Task<MediaStatus> QueueInsertAsync(QueueItem[] items, long? insertBefore = null)
        {
            return await SendAsync(new QueueInsertMessage() { Items = items, InsertBefore = insertBefore },
                SharpcasteSerializationContext.Default.QueueInsertMessage);
        }

        public async Task<MediaStatus> QueueRemoveAsync(long[] itemIds)
        {
            return await SendAsync(new QueueRemoveMessage() { ItemIds = itemIds },
                SharpcasteSerializationContext.Default.QueueRemoveMessage);
        }

        public async Task<MediaStatus> QueueReorderAsync(long[] itemIds, long? insertBefore = null)
        {
            return await SendAsync(new QueueReorderMessage() { ItemIds = itemIds, InsertBefore = insertBefore },
                SharpcasteSerializationContext.Default.QueueReorderMessage);
        }

        public async Task<MediaStatus> QueueUpdateAsync(QueueItem[] items)
        {
            return await SendAsync(new QueueUpdateMessage() { Items = items },
                SharpcasteSerializationContext.Default.QueueUpdateMessage);
        }

        public async Task<QueueItem[]> QueueGetItemsAsync(int[] ids = null)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            var queueGetItemsMessage = new QueueGetItemsMessage() { MediaSessionId = MediaStatus.MediaSessionId, Ids = ids };
            var response = await SendAsync(queueGetItemsMessage.RequestId, JsonSerializer.Serialize(queueGetItemsMessage, SharpcasteSerializationContext.Default.QueueGetItemsMessage), chromecastStatus.Application.TransportId);
            var queueItemsResponse = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.QueueItemsMessage);
            return queueItemsResponse.Items;
        }

        public async Task<int[]> QueueGetItemIdsAsync()
        {
            var chromecastStatus = Client.GetChromecastStatus();
            var queueGetItemIdsMessage = new QueueGetItemIdsMessage() { MediaSessionId = MediaStatus.MediaSessionId };
            var response = await SendAsync(queueGetItemIdsMessage.RequestId, JsonSerializer.Serialize(queueGetItemIdsMessage, SharpcasteSerializationContext.Default.QueueGetItemIdsMessage), chromecastStatus.Application.TransportId);
            var queueItemIdsResponse = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.QueueItemIdsMessage);
            return queueItemIdsResponse.Ids;
        }

        public async Task<MediaStatus> GetMediaStatusAsync()
        {
            var chromecastStatus = Client.GetChromecastStatus();
            var mediaStatusMessage = new GetStatusMessage();
            var response = await SendAsync(mediaStatusMessage.RequestId, JsonSerializer.Serialize(mediaStatusMessage, SharpcasteSerializationContext.Default.GetStatusMessage), chromecastStatus.Application.TransportId);
            var mediaStatus = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
            return mediaStatus.Status.FirstOrDefault();
        }
    }
}
