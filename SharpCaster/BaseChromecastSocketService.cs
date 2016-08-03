using System;
using System.IO;
using SharpCaster.Models;
using System.Collections.Generic;
using SharpCaster.Extensions;
using System.Linq;
using System.Diagnostics;

namespace SharpCaster
{
    public abstract class BaseChromecastSocketService
    {
        public List<ChromecastChannel> Channels { get; set; }

        public BaseChromecastSocketService()
        {
            Channels = new List<ChromecastChannel>();
        }

        protected void ReadPacket(Stream stream, bool parsed)
        {
            try
            {
                IEnumerable<byte> entireMessage;
                if (parsed)
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    entireMessage = buffer;
                }
                else
                {
                    entireMessage = stream.ParseData();
                }

                var entireMessageArray = entireMessage.ToArray();
                var castMessage = entireMessageArray.ToCastMessage();
                if (castMessage == null) return;
                Debug.WriteLine("Received: " + castMessage.GetJsonType());
                if (string.IsNullOrEmpty(castMessage.Namespace)) return;
                ReceivedMessage(castMessage);
            }
            catch (Exception ex)
            {
                // Log these bytes
                Debug.WriteLine(ex);
            }

        }

        private void ReceivedMessage(CastMessage castMessage)
        {
            foreach (var channel in Channels.Where(i => i.Namespace == castMessage.Namespace))
            {
                channel.OnMessageReceived(new ChromecastSSLClientDataReceivedArgs(castMessage));
            }
        }
    }
}