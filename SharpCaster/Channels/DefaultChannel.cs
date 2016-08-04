using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Channels
{
    public class DefaultChannel : ChromecastChannel 
    {
        public DefaultChannel(ChromeCastClient client, string ns) : base(client, ns)
        {
        }
    }
}
