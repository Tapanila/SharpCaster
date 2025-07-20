using Sharpcaster.Converters;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    public class QueueLoadMessage : MessageWithSession
    {
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }

        /// <summary>
        /// The index of the item in the items array that must be the first currentItem (the item that will be played first).
        /// Note this is the index of the array (starts at 0) and not the itemId (as it is not known until the queue is created).
        /// If repeatMode is REPEAT_OFF playback will end when the last item in the array is played (elements before the startIndex will not be played).
        /// This may be useful for continuation scenarios where the user was already using the sender app and in the middle decides to cast.
        /// In this way the sender app does not need to map between the local and remote queue positions or saves one extra QUEUE_UPDATE request.
        /// </summary>
        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; } = 0;
        /// <summary>
        /// Behavior of the queue when all items have been played.
        /// </summary>
        [JsonPropertyName("repeatMode")]
        [JsonConverter(typeof(RepeatModeEnumConverter))]
        public RepeatModeType RepeatMode { get; set; }

    }
}
