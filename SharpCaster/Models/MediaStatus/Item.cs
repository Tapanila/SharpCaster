namespace SharpCaster.Models.MediaStatus
{
    public class Item
    {
        public int itemId { get; set; }
        public Media media { get; set; }
        public bool autoplay { get; set; }
    }
}