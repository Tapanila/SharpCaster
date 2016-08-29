using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]

    public class VolumeDataObject
    {
        [DataMember(Name = "level")]
        public double? Level { get; set; }

        [DataMember(Name = "muted")]
        public bool? Muted { get; set; }
    }
}