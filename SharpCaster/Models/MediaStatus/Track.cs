namespace SharpCaster.Models.MediaStatus
{
    public class Track
    {
        public int trackId { get; set; }
        public string trackContentType { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public object language { get; set; }
    }
}