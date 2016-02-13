using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    public class SeekRequest : MediaRequest
    {
        public SeekRequest(long mediaSessionId, double seconds) : base("SEEK", mediaSessionId)
        {
            CurrentTime = seconds;
        }

        [DataMember(Name = "currentTime")] public double CurrentTime { get; set; }
    }
}