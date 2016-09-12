using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class Metadata
    {
        public Metadata(int metadataType = 0)
        {
            MetadataType = metadataType;
        }

        [DataMember(Name = "metadataType")]
        public int MetadataType { get; set; }

        [IgnoreDataMember]
        public virtual string ContentType { get; set; }
    }
}