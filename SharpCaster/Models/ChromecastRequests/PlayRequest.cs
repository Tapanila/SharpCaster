using System.Runtime.Serialization;

namespace SharpCaster.Models.ChromecastRequests
{
    [DataContract]
    public class PlayRequest : RequestWithId
    {
        public PlayRequest()
            : base("PLAY")
        {
        }
    }
}