using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class VolumeRequest : RequestWithId
    {
        public VolumeRequest(double level, int? requestId = null) : base("SET_VOLUME", requestId)
        {
            VolumeDataObject = new VolumeDataObject {Level = level};
        }
        public VolumeRequest(bool muted, int? requestId = null) : base("SET_VOLUME", requestId)
        {
            VolumeDataObject = new VolumeDataObject { Muted = muted };
        }

        [DataMember(Name = "volume")]
        public VolumeDataObject VolumeDataObject { get; set; }

    }
}