using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class VolumeRequest : RequestWithId
    {
        public VolumeRequest(float level) : base("SET_VOLUME")
        {
            VolumeDataObject = new VolumeDataObject {Level = level};
        }
        public VolumeRequest(bool muted) : base("SET_VOLUME")
        {
            VolumeDataObject = new VolumeDataObject { Muted = muted };
        }

        [DataMember(Name = "volume")]
        public VolumeDataObject VolumeDataObject { get; set; }

    }
}