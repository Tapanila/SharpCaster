using System.Runtime.Serialization;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Models.Metadata;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class MediaData
    {
        public MediaData(string url, string contentType, IMetadata metadata = null, StreamType streamType = StreamType.BUFFERED, double duration = 0d, object customData = null, Track[] tracks = null)
        {
            Url = url;
            ContentType = contentType;
            StreamType = streamType;
            Duration = duration;
            Metadata = metadata;
            CustomData = customData;
            Tracks = tracks;
        }

        [DataMember(Name = "contentId")]
        public string Url { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(Name = "metadata")]
        public IMetadata Metadata { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets or sets the type of the stream. This can be BUFFERED, LIVE or NONE
        /// </summary>
        ///
        /// <value>
        ///     The type of the stream.
        /// </value>
        ///-------------------------------------------------------------------------------------------------
        [DataMember(Name = "streamType")]
        [JsonConverter(typeof(StreamTypeConverter))]
        public StreamType StreamType { get; set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "customData")]
        public object CustomData { get; set; }

        [DataMember(Name = "tracks")]
        public Track[] Tracks { get; set; }
    }

    public enum StreamType
    {
        NONE,
        BUFFERED,
        LIVE
    }
}