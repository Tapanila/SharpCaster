namespace Sharpcaster.Messages.Media
{
    public class StopMediaMessage : MediaSessionMessage
    {
        public StopMediaMessage()
        {
            Type = "STOP_MESSAGE";
        }
    }
}
