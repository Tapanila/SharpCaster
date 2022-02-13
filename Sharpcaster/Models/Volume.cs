using System.Runtime.Serialization;

namespace Sharpcaster.Models
{
    [DataContract]
    public class Volume
    {
        [DataMember(Name = "level")]
        public double? Level { get; set; }

        [DataMember(Name = "muted")]
        public bool? Muted { get; set; }
    }
}
