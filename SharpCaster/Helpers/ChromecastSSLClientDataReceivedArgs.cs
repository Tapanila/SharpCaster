using System;

namespace SharpCaster.Helpers
{
    public class ChromecastSSLClientDataReceivedArgs : EventArgs
    {
        public ChromecastSSLClientDataReceivedArgs(CastMessage message)
        {
            Message = message;
        }
        public CastMessage Message { get; set; }
    }
}