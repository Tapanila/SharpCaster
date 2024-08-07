﻿using System.Runtime.Serialization;
using Sharpcaster.Messages;

namespace Sharpcaster.Test.customChannel
{
    [DataContract]
    internal class WebMessage : MessageWithSession
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}