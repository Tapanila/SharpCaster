using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class MusicTrackMediaMetadata : Metadata
    {
        [DataMember(Name = "albumName")]
        public string AlbumName { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "albumArtist")]
        public string AlbumArtist { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "images")]
        public List<ImageData> Images { get; set; }

        [IgnoreDataMember]
        public override string ContentType
        {
            get { return "audio/mp3"; }
        }

        public MusicTrackMediaMetadata() : base(3)
        {
            Images = new List<ImageData>();
        }
    }
}