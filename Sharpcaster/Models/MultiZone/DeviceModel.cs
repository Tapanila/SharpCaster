using System.Runtime.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    [DataContract]
    public class Device
    {
        [DataMember(Name = "capabilities")]
        public string Capabilities { get; set; }

        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [DataMember(Name = "volume")]
        public Volume Volume { get; set; }
    }
}
