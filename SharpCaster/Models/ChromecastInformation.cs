using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Models
{
    public class ChromecastInformation
    {
        public Uri DeviceUri { get; set; }
        public string FriendlyName { get; set; }
        public Guid SsdpUuid { get; internal set; }
    }
}
