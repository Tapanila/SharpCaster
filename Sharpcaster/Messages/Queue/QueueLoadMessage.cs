using Newtonsoft.Json;
using Sharpcaster.Converters;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [DataContract]
    public class QueueLoadMessage : MessageWithSession
    {
        [DataMember(Name = "items")]
        public QueueItem[] Items { get; set; }

        /// <summary>
        /// The index of the item in the items array that must be the first currentItem (the item that will be played first).
        /// Note this is the index of the array (starts at 0) and not the itemId (as it is not known until the queue is created).
        /// If repeatMode is REPEAT_OFF playback will end when the last item in the array is played (elements before the startIndex will not be played).
        /// This may be useful for continuation scenarios where the user was already using the sender app and in the middle decides to cast.
        /// In this way the sender app does not need to map between the local and remote queue positions or saves one extra QUEUE_UPDATE request.
        /// </summary>
        [DataMember(Name = "startIndex")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? StartIndex { get; set; }
        /// <summary>
        /// Behavior of the queue when all items have been played.
        /// </summary>

        [DataMember(Name = "repeatMode")]
        [JsonConverter(typeof(RepeatModeEnumConverter))]
        public RepeatModeType RepeatMode { get; set; }

        /// <summary>
        /// Seconds (since the beginning of content) to start playback of the first item to be played.
        /// If provided, this value will take precedence over the startTime value provided at the QueueItem level but only the first time the item is played.
        /// This is to cover the common case where the user casts the item that was playing locally so the currentTime does not apply to the item permanently like the QueueItem startTime does.
        /// It avoids having to reset the startTime dynamically (that may not be possible if the phone has gone to sleep).
        /// </summary>

        [DataMember(Name = "currentTime")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? CurrentTime { get; set; }
    }
}
