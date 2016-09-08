using System.Runtime.Serialization;

namespace SharpCaster.Models.MediaStatus
{
    [DataContract]
    public class Track
    {
        [DataMember(Name = "customData")]
        public object CustomData { get; set; }
        [DataMember(Name = "language")]
        public object Language { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "subtype")]
        public string SubType { get; set; }
        [DataMember(Name = "trackContentId")]
        public string TrackContentId { get; set; }
        [DataMember(Name = "trackContentType")]
        public string TrackContentType { get; set; }
        [DataMember(Name = "trackId")]
        public long TrackId { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}