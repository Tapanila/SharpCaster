using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    public class SeekRequest : MediaRequest
    {
        public SeekRequest(long mediaSessionId, double seconds, int? requestId = null) 
            : base("SEEK", mediaSessionId,requestId)
        {
            CurrentTime = seconds;
        }

        [DataMember(Name = "currentTime")] public double CurrentTime { get; set; }
    }
}