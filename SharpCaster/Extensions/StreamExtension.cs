using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpCaster.Extensions
{
    public static class StreamExtension
    {
        public static IEnumerable<byte> ParseData(this Stream stream)
        {
            var buffer = new byte[2048];

            var header = new List<byte>();
            var data = new List<byte>();


            var escape = true;

            while (escape)
            {
                // tricky byte order for messages
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 1)
                {
                    // Incoming series of header /data
                    header.Add(buffer[0]);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 3)
                    {
                        header.Add(buffer[0]);
                        header.Add(buffer[1]);
                        header.Add(buffer[2]);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 1)
                        {
                            header.Add(buffer[0]);
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            data.AddRange(buffer.Take(bytesRead));
                            escape = false;
                        }
                        else
                        {
                            escape = false;
                        }
                    }
                    else
                    {
                        escape = false;
                    }
                }
                else
                {
                    escape = false;
                }
            }

            var entireMessage = new List<byte>();
            entireMessage.AddRange(header);
            entireMessage.AddRange(data);

            return entireMessage;
        }
    }
}
