using System.Collections.Generic;
using System.Linq;

namespace SharpCaster.Channels
{
    public class PlexChannel : MediaChannel
    {
        public static string Urn = "urn:x-cast:plex";
        public PlexChannel(ChromeCastClient client) : base(client, Urn)
        {
        }

        //public async Task SkipTo()
        //{
        //    throw new NotImplementedException();
        //{
        //    "type": "SKIPTO",
        //    "key": unknown, probably something like "/library/metadata/2769"
        //}
        //}

        //public async Task ShowDetails()
        //{
        //    throw new NotImplementedException();
        //    //unknown
        //}

        //public async Task RefreshPlayQueue()
        //{
        //    throw new NotImplementedException();
        //    //unknown

        //00 00 00 4E 08 00 12 12  31 34 37 31 38 39 35 30      N. ..14718950
        //32 38 33 38 36 33 33 39  32 34 1A 06 77 65 62 2D   2838633924..web-
        //31 37 22 0F 75 72 6E 3A  78 2D 63 61 73 74 3A 70   17".urn:x-cast:p
        //6C 65 78 28 00 32 1B 7B  22 74 79 70 65 22 3A 22   lex( 2.{"type":"
        //52 45 46 52 45 53 48 50  4C 41 59 51 55 45 55 45   REFRESHPLAYQUEUE
        //22 7D                                              "}

        //{
        //    "type": "REFRESHPLAYQUEUE"
        //}
        //}

        /// <summary>
        /// Set the bitrate
        /// </summary>
        /// <param name="bitrate">Bitrate in kBit</param>
        /// <remarks>-1 is used for original quality. Options are <list type="int">
        /// <value>-1</value>
        /// <value>64</value>
        /// <value>96</value>
        /// <value>208</value>
        /// <value>320</value>
        /// <value>720</value>
        /// <value>1500</value>
        /// <value>2000</value>
        /// <value>3000</value>
        /// <value>4000</value>
        /// <value>8000</value>
        /// <value>10000</value>
        /// <value>12000</value>
        /// <value>20000</value>
        /// </list></remarks>
        /// <returns></returns>
        //public async Task SetQuality(int bitrate)
        //{
        //    throw new NotImplementedException();

        //    //{
        //    //    "type": "SETQUALITY",
        //    //    "bitrate": int in kBit (64, 96, 208, 320, 720, 1500, 2e3, 3e3, 4e3, 8e3, 1e4, 12e3, 2e4)
        //    //}
        //}

        /// <summary>
        /// Used to select a video stream, audio stream, subtitle stream or lyrics stream 
        /// </summary>
        /// <remarks>This is the case when there are e.g. multiple languages in one contrainer</remarks>
        /// <returns></returns>
        //public async Task SetStream(string type, int streamId)
        //{
        //    throw new NotImplementedException();

        //    //{
        //    //    "type": "SETSTREAM",
        //    //    "stream": {
        //    //        "type":"subtitles", // video | audio | subtitles | lyrics
        //    //        "id":482 // stream id (selected from metadata)
        //    //        }
        //    //}

        //    //00 00 00 6E 08 00 12 12  31 34 37 31 38 39 35 30      n. ..14718950
        //    //32 38 33 38 36 33 33 39  32 34 1A 06 77 65 62 2D   2838633924..web-
        //    //31 34 22 0F 75 72 6E 3A  78 2D 63 61 73 74 3A 70   14".urn:x-cast:p
        //    //6C 65 78 28 00 32 3B 7B  22 74 79 70 65 22 3A 22   lex( 2;{"type":"
        //    //53 45 54 53 54 52 45 41  4D 22 2C 22 73 74 72 65   SETSTREAM","stre
        //    //61 6D 22 3A 7B 22 74 79  70 65 22 3A 22 73 75 62   am":{"type":"sub
        //    //74 69 74 6C 65 73 22 2C  22 69 64 22 3A 34 38 32   titles","id":482
        //    //7D 7D                                              }}
        //}
    }

    public static class PlexChannelExtension
    {
        public static PlexChannel GetPlexChannel(this IEnumerable<IChromecastChannel> channels)
        {
            return (PlexChannel)channels.First(x => x.Namespace == PlexChannel.Urn);
        }
    }
}
