using System.Collections.Generic;

namespace SharpCaster.Models.ChromecastStatus
{
    public class Status
    {
        public List<ChromecastApplication> applications { get; set; }
        public bool isActiveInput { get; set; }
        public bool isStandBy { get; set; }
        public Volume volume { get; set; }
    }
}