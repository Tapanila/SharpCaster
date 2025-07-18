﻿using Sharpcaster.Models;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    public class SetVolumeMessage : MessageWithId
    {
        [JsonPropertyName("volume")]
        public Volume Volume { get; set; }
    }
}
