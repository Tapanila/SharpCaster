﻿using System.Collections.Generic;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Models.Metadata
{
    //Fields: https://developers.google.com/cast/docs/reference/chrome/chrome.cast.media.MusicTrackMediaMetadata
    public class MusicTrackMediaMetadata : IMetadata
    {
        public string albumArtist { get; set; }
        public string albumName { get; set; }
        public string artist { get; set; }
        public string composer { get; set; }
        public int discNumber { get; set; }
        public List<ChromecastImage> images { get; set; }
        public int metadataType { get; set; }
        public string releaseDate { get; set; }
        public string songName { get; set; }
        public string title { get; set; }
        public int trackNumber { get; set; }
    }
}