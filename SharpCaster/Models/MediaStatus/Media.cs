namespace SharpCaster.Models.MediaStatus
{
    public class Media
    {
        public string contentId { get; set; }
        public string contentType { get; set; }
        public string streamType { get; set; }
        public double duration { get; set; }
        public GenericMediaMetadata metadata { get; set; }
        public object customData { get; set; }
    }
}