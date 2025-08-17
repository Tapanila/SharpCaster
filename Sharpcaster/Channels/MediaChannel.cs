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
    public class MediaChannel : ChromecastChannel
    {
        /// <summary>
        /// Raised when error is received
        /// </summary>
        public event EventHandler<ErrorMessage>? ErrorHappened;
        public event EventHandler<LoadCancelledMessage>? LoadCancelled;
        public event EventHandler<LoadFailedMessage>? LoadFailed;
        public event EventHandler<InvalidRequestMessage>? InvalidRequest;
        public event EventHandler<MediaStatus>? StatusChanged;

        public MediaStatus? MediaStatus { get => mediaStatus; }
        private MediaStatus? mediaStatus;

        private static readonly Action<ILogger, string, Exception?> LogErrorSendingMessage =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(2001, "SendMessageError"), "Error sending message: {ExceptionMessage}");
        /// <summary>
        /// Initializes a new instance of MediaChannel class
        /// </summary>
        public MediaChannel(ILogger? logger = null) : base("media", logger)
        {
        }

        private async Task<MediaStatus?> SendAsync(int messageRequestId, string messagePayload, ChromecastApplication application, bool DoNotReturnOnLoading = true)
        {
            try
            {
                var response = await SendAsync(messageRequestId, messagePayload, application.TransportId).ConfigureAwait(false);
                var mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
                if (DoNotReturnOnLoading && mediaStatusMessage?.Status?.FirstOrDefault()?.ExtendedStatus?.PlayerState == PlayerStateType.Loading)
                {
                    response = await Client.WaitResponseAsync(messageRequestId).ConfigureAwait(false);
                    mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
                }
                mediaStatus = mediaStatusMessage?.Status?.FirstOrDefault();
                return mediaStatus;
            }
            catch (Exception ex)
            {
                if (Logger != null) LogErrorSendingMessage(Logger, ex.Message, ex);
                mediaStatus = null;
                throw;
            }
        }

        private async Task<MediaStatus?> SendAsync<T>(T message, JsonTypeInfo<T> serializationContext, bool DoNotReturnOnLoading = true) where T : MediaSessionMessage
        {
            var chromecastStatus = Client.ChromecastStatus;
            message.MediaSessionId = MediaStatus?.MediaSessionId ?? throw new InvalidOperationException("MediaSessionID is not available");
            var messagePayload = JsonSerializer.Serialize(message, serializationContext);
            return await SendAsync(message.RequestId, messagePayload, chromecastStatus.Applications[0], DoNotReturnOnLoading).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads a media with specific active tracks
        /// </summary>
        /// <param name="media">media to load</param>
        /// <param name="autoPlay">true to play the media directly, false otherwise</param>
        /// <param name="activeTrackIds">array of track IDs that should be active</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> LoadAsync(Media media, bool autoPlay = true, int[]? activeTrackIds = null)
        {
            var status = Client.ChromecastStatus;
            var loadMessage = new LoadMessage() 
            { 
                SessionId = status.Application.SessionId, 
                Media = media, 
                AutoPlay = autoPlay,
                ActiveTrackIds = activeTrackIds
            };
            return await SendAsync(loadMessage.RequestId, JsonSerializer.Serialize(loadMessage, SharpcasteSerializationContext.Default.LoadMessage), status.Application).ConfigureAwait(false);
        }

        public override void OnMessageReceived(string messagePayload, string type)
        {
            switch (type)
            {
                case "LOAD_FAILED":
                    var loadFailedMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.LoadFailedMessage);
                    if (loadFailedMessage != null)
                    {
                        SafeInvokeEvent(LoadFailed, this, loadFailedMessage);
                    }
                    return;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.LoadCancelledMessage);
                    if (loadCancelledMessage != null)
                    {
                        SafeInvokeEvent(LoadCancelled, this, loadCancelledMessage);
                    }
                    return;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.InvalidRequestMessage);
                    if (invalidRequestMessage != null)
                    {
                        SafeInvokeEvent(InvalidRequest, this, invalidRequestMessage);
                    }
                    return;
                case "ERROR":
                    var errorMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.ErrorMessage);
                    if (errorMessage != null)
                    {
                        SafeInvokeEvent(ErrorHappened, this, errorMessage);
                    }
                    return;
                case "MEDIA_STATUS":
                    var mediaStatusMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.MediaStatusMessage);
                    mediaStatus = mediaStatusMessage?.Status.FirstOrDefault();
                    if (MediaStatus != null)
                    {
                        SafeInvokeEvent(StatusChanged, this, MediaStatus);
                    }
                    return;
                case "QUEUE_ITEMS":
                //{"type":"QUEUE_ITEMS","requestId":908492678,"items":[{"itemId":9,"media":{"contentId":"Aquarium","contentUrl":"https://incompetech.com/music/royalty-free/mp3-royaltyfree/Aquarium.mp3","streamType":2,"contentType":"audio/mpeg","mediaCategory":"AUDIO","duration":144.013078},"orderId":0}],"sequenceNumber":0}
                case "QUEUE_ITEM_IDS":
                case "QUEUE_CHANGE":
                    return;
            }
#if DEBUG
            Debugger.Break();
#endif
            return;
        }

        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> PlayAsync()
        {
            return await SendAsync(new PlayMessage(), SharpcasteSerializationContext.Default.PlayMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> PauseAsync()
        {
            return await SendAsync(new PauseMessage(), SharpcasteSerializationContext.Default.PauseMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> StopAsync()
        {
            return await SendAsync(new StopMediaMessage(), SharpcasteSerializationContext.Default.StopMediaMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Seeks to the specified time
        /// </summary>
        /// <param name="seconds">time in seconds</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> SeekAsync(double seconds)
        {
            return await SendAsync(new SeekMessage() { CurrentTime = seconds }, SharpcasteSerializationContext.Default.SeekMessage).ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueueLoadAsync(QueueItem[] items, RepeatModeType repeatMode = RepeatModeType.OFF, int startIndex = 0)
        {
            var chromecastStatus = Client.ChromecastStatus;
            var queueLoadMessage = new QueueLoadMessage() { SessionId = chromecastStatus.Application.SessionId, Items = items, RepeatMode = repeatMode, StartIndex = startIndex };
            var response = await SendAsync(queueLoadMessage.RequestId, JsonSerializer.Serialize(queueLoadMessage, SharpcasteSerializationContext.Default.QueueLoadMessage), chromecastStatus.Application.TransportId).ConfigureAwait(false);
            var mediaStatusMessage = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
            if (mediaStatusMessage != null && mediaStatusMessage.Status != null)
            {
                return mediaStatusMessage.Status.FirstOrDefault();
            }
            return null;
        }

        public async Task<MediaStatus?> QueueNextAsync()
        {
            //For some reason the request id is only returned with the first message that might be buffering, idle or playing and rest of the messages are without it
            await SendAsync(new QueueUpdateMessage { Jump = 1 }, SharpcasteSerializationContext.Default.QueueUpdateMessage, false).ConfigureAwait(false);
            return await GetMediaStatusAsync().ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueuePrevAsync()
        {
            //For some reason the request id is only returned with the first message that might be buffering, idle or playing and rest of the messages are without it
            await SendAsync(new QueueUpdateMessage { Jump = -1}, SharpcasteSerializationContext.Default.QueueUpdateMessage, false).ConfigureAwait(false);
            return await GetMediaStatusAsync().ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueueInsertAsync(QueueItem[] items, int? insertBefore = null)
        {
            return await SendAsync(new QueueInsertMessage() { Items = items, InsertBefore = insertBefore },
                SharpcasteSerializationContext.Default.QueueInsertMessage).ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueueRemoveAsync(int[] itemIds)
        {
            return await SendAsync(new QueueRemoveMessage() { ItemIds = itemIds },
                SharpcasteSerializationContext.Default.QueueRemoveMessage).ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueueReorderAsync(int[] itemIds, int? insertBefore = null)
        {
            return await SendAsync(new QueueReorderMessage() { ItemIds = itemIds, InsertBefore = insertBefore },
                SharpcasteSerializationContext.Default.QueueReorderMessage).ConfigureAwait(false);
        }

        public async Task<MediaStatus?> QueueUpdateAsync(QueueItem[] items)
        {
            return await SendAsync(new QueueUpdateMessage() { Items = items },
                SharpcasteSerializationContext.Default.QueueUpdateMessage).ConfigureAwait(false);
        }

        public async Task<QueueItem[]?> QueueGetItemsAsync(int[]? ids = null)
        {
            var chromecastStatus = Client.ChromecastStatus;
            var queueGetItemsMessage = new QueueGetItemsMessage() { MediaSessionId = MediaStatus?.MediaSessionId ?? throw new InvalidOperationException("MediaSessionID is not available"), Ids = ids };
            var response = await SendAsync(queueGetItemsMessage.RequestId, JsonSerializer.Serialize(queueGetItemsMessage, SharpcasteSerializationContext.Default.QueueGetItemsMessage), chromecastStatus.Application.TransportId).ConfigureAwait(false);
            var queueItemsResponse = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.QueueItemsMessage);
            return queueItemsResponse?.Items;
        }

        public async Task<int[]?> QueueGetItemIdsAsync()
        {
            var chromecastStatus = Client.ChromecastStatus;
            var queueGetItemIdsMessage = new QueueGetItemIdsMessage() { MediaSessionId = MediaStatus?.MediaSessionId ?? throw new InvalidOperationException("MediaSessionID is not available") };
            var response = await SendAsync(queueGetItemIdsMessage.RequestId, JsonSerializer.Serialize(queueGetItemIdsMessage, SharpcasteSerializationContext.Default.QueueGetItemIdsMessage), chromecastStatus.Application.TransportId).ConfigureAwait(false);
            var queueItemIdsResponse = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.QueueItemIdsMessage);
            return queueItemIdsResponse?.Ids;
        }

        public async Task<MediaStatus?> GetMediaStatusAsync()
        {
            var chromecastStatus = Client.ChromecastStatus;
            var mediaStatusMessage = new GetStatusMessage();
            var response = await SendAsync(mediaStatusMessage.RequestId, JsonSerializer.Serialize(mediaStatusMessage, SharpcasteSerializationContext.Default.GetStatusMessage), chromecastStatus.Application.TransportId).ConfigureAwait(false);
            var mediaStatus = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.MediaStatusMessage);
            return mediaStatus?.Status?.FirstOrDefault();
        }

        /// <summary>
        /// Skips the current ad
        /// </summary>
        /// <returns>media status</returns>
        [Obsolete("SkipAd doesn't work. If you need this open a Github issue.", true)]
        public async Task<MediaStatus?> SkipAdAsync()
        {
            return await SendAsync(new SkipAdMessage(), SharpcasteSerializationContext.Default.SkipAdMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Shuffles the queue
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> QueueShuffleAsync(bool shuffle = true)
        {
            return await SendAsync(new QueueUpdateMessage { Shuffle = shuffle }, SharpcasteSerializationContext.Default.QueueUpdateMessage, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the queue repeat mode
        /// </summary>
        /// <param name="repeatMode">repeat mode to set</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> QueueSetRepeatModeAsync(RepeatModeType repeatMode)
        {
            return await SendAsync(new QueueUpdateMessage { RepeatMode = repeatMode }, SharpcasteSerializationContext.Default.QueueUpdateMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the playback rate
        /// </summary>
        /// <param name="playbackRate">playback rate (e.g., 0.5 for half speed, 2.0 for double speed)</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> SetPlaybackRateAsync(double playbackRate)
        {
            return await SendAsync(new SetPlaybackRateMessage() { PlaybackRate = playbackRate }, SharpcasteSerializationContext.Default.SetPlaybackRateMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a user action
        /// </summary>
        /// <param name="userAction">user action to send</param>
        /// <returns>media status</returns>
        [Obsolete("SenUserAction doesn't work. If you need this open a Github issue.", true)]
        public async Task SendUserActionAsync(UserAction userAction)
        {
            var userActionMessage = new UserActionMessage() { UserAction = userAction };
            var message = JsonSerializer.Serialize(userActionMessage, SharpcasteSerializationContext.Default.UserActionMessage);
            await SendAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Edits track information
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> EditTracksAsync(int[]? activeTrackIds = null, TextTrackStyle? textTrackStyle = null, string? language = null, object? customData = null)
        {
            return await SendAsync(new EditTracksInfoMessage () { ActiveTrackIds = activeTrackIds, TextTrackStyle = textTrackStyle, Language = language, CustomData = customData }, SharpcasteSerializationContext.Default.EditTracksInfoMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the media stream volume level
        /// </summary>
        /// <param name="level">volume level between 0.0 and 1.0</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> SetVolumeAsync(double level)
        {
            if (level < 0 || level > 1.0)
            {
                throw new ArgumentException("level must be between 0.0 and 1.0", nameof(level));
            }
            var status = await SendAsync(new SetVolumeMessage() { Volume = new Models.Volume { Level = level } }, SharpcasteSerializationContext.Default.SetVolumeMessage).ConfigureAwait(false);
            if (status?.Volume?.Level == null || Math.Abs(status.Volume.Level.Value - level) > 0.1)
            {
                if (status?.Volume != null)
                    Logger.LogDebug("Volume level is {currentVolume} and it was supposed to be {newLevel}", status.Volume.Level, level);

                status = await SendAsync(new SetVolumeMessage() { Volume = new Models.Volume { Level = level } }, SharpcasteSerializationContext.Default.SetVolumeMessage).ConfigureAwait(false);
            }
            return status;
        }

        /// <summary>
        /// Sets the media stream mute state
        /// </summary>
        /// <param name="muted">true to mute, false to unmute</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus?> SetMuteAsync(bool muted)
        {
            return await SendAsync(new SetVolumeMessage() { Volume = new Models.Volume { Muted = muted } }, SharpcasteSerializationContext.Default.SetVolumeMessage).ConfigureAwait(false);
        }
    }
}
