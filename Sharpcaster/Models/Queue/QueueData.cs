using Sharpcaster.Models.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.Models.Queue
{
    [DataContract]
    public class QueueData
    {
        /// <summary>
        /// The description of the queue.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }
        /// <summary>
        /// An optional Queue entity ID, providing a Google Assistant deep link.
        /// </summary>
        [DataMember(Name = "entity")]
        public string Entity { get; set; }
        /// <summary>
        /// The ID of the queue.
        /// </summary>
        [DataMember(Name ="id")]
        public string Id { get; set; }
        /// <summary>
        /// An Array of queue items, sorted in playback order.
        /// </summary>
        [DataMember(Name ="items")]
        public QueueItem[] Items { get; set; }
        /// <summary>
        /// The name of the queue.
        /// </summary>
        [DataMember(Name ="name")]
        public string Name { get; set; }
        /// <summary>
        /// A queue type, such as album, playlist, radio station, or tv series.
        /// </summary>
        [DataMember(Name= "queueType")]
        public string QueueType { get; set; }
        /// <summary>
        /// The continuous playback behavior of the queue.
        /// </summary>
        [DataMember(Name = "repeatMode")]
        public string RepeatMode { get; set; }
        /// <summary>
        /// True indicates that the queue is shuffled.
        /// </summary>
        [DataMember(Name = "shuffle")]
        public bool IsShuffle { get; set; }
        /// <summary>
        /// The index of the item in the queue that should be used to start playback first.
        /// </summary>
        [DataMember(Name = "startIndex")]
        public long? StartIndex { get; set; }
        /// <summary>
        /// When to start playback of the first item, expressed as the number of seconds since the beginning of the media.
        /// </summary>
        [DataMember(Name = "startTime")]
        public long? StartTime { get; set; }
    }
}