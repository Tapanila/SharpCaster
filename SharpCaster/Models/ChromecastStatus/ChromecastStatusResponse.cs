namespace SharpCaster.Models.ChromecastStatus
{
    public class ChromecastStatusResponse
    {
        public int requestId { get; set; }
        public Status status { get; set; }
        public string type { get; set; }
    }
}