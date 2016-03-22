using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;

namespace SharpCaster.Extensions
{
    public static class StreamSocketExtension
    {
        public static StreamSocket ConfigureForChromecast(this StreamSocket socket)
        {
            //Chromecast is not using trusted certificate so ignoring errors caused by that
            socket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            socket.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            socket.Control.OutboundBufferSizeInBytes = 2048;

            socket.Control.KeepAlive = true;
            socket.Control.QualityOfService = SocketQualityOfService.LowLatency;
            return socket;
        }
    }
}
