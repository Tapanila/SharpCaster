using Microsoft.Extensions.Configuration;
using Sharpcaster.Models.Media;
using SharpCaster.Console.Controllers;
using SharpCaster.Console.Models;
using SharpCaster.Console.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Console.Services
{
    public class PlaylistService
    {
        private Category _playlistTree;
        private Dictionary<string, List<Media>> _Playlists = new Dictionary<string, List<Media>>();

        public PlaylistService(ApplicationState state, DeviceService deviceService, IConfiguration config)
        {
            _playlistTree = new Category("playlists");
            var _plConfig = config.GetSection("Playlists").GetSection("Content");

            AddPlaylists(_playlistTree, _plConfig);
        }

        public bool HasContent()
        {
            return _playlistTree != null;
        }

        public MenuNode GetRoot()
        {
            return _playlistTree;
        }
        public bool IsPlaylistId(string arg)
        {
            return _Playlists.Keys.Contains(arg);
        }

        private void AddPlaylists(Category parent, IConfigurationSection playlists)
        {
            // playlists.Bind(parent); This does not work with subclasses node/category/playlist etc.
            //                         And to my understabding its not AOT safe.

            // Parse the config sections recursively. Not using GetValues() or Bind() for avoiding AOT issues ...
            List<MenuNode> entries = new List<MenuNode>();
            var subsections = playlists.GetChildren();
            foreach (var section in subsections)
            {
                // Check if this node is a playlist (has a "Tracks" subsection)
                var trackSection = section.GetChildren().Where(s => s.Key.Equals("Tracks")).ToList().FirstOrDefault();
                if (trackSection != null)
                {
                    var playlist = new Playlist(section["Name"]??"NoName", section["Id"]);
                    var tracks = trackSection?.GetChildren().AsEnumerable();
                    if (tracks != null)
                    {
                        foreach (var track in tracks)
                        {
                            Media media = new Media();
                            var keyValues = track.GetChildren().AsEnumerable();
                            foreach (var kv in keyValues)
                            {
                                AddConfigValue(media, kv);
                            }
                            if (media.ContentUrl == null && media.ContentId == null)
                            {
                                AnsiConsole.MarkupLine("[yellow]⚠️  Warning: Skipping media item with no ContentUrl or ContentId in playlist: [/]" + playlist.Name);
                            }
                            else
                            {
                                if (media.ContentId != null)
                                {
                                    if (_Playlists.ContainsKey(media.ContentId))
                                    {
                                        _Playlists[media.ContentId].Add(media);
                                    }
                                    else
                                    {
                                        _Playlists[media.ContentId] = new List<Media>() { media };
                                    }
                                }
                            }
                            playlist.Tracks.Add(media);
                        }
                    }
                    entries.Add(playlist);
                }
                else
                {
                    // Check if this node is a Category (has a "Content" subsection)    
                    var contentSection = section.GetChildren().Where(s => s.Key.Equals("Content")).FirstOrDefault();
                    // This section contains subsections -> create a new node and recurse
                    if (contentSection != null)
                    {
                        var container = new Category(section["Name"]??"Unknown");
                        AddPlaylists(container, contentSection);
                        entries.Add(container);
                    }
                }

            }
            parent.Content.AddRange(entries);
        }

        private void AddConfigValue(Media media, IConfigurationSection kv)
        {
            switch (kv.Key)
            {
                case "ContentId":
                    media.ContentId = kv.Value?.ToString();
                    break;
                case "ContentUrl":
                    media.ContentUrl = kv.Value?.ToString();
                    break;
                case "Title":
                    media.Metadata = media.Metadata ?? new MediaMetadata();
                    media.Metadata.Title = kv.Value?.ToString();
                    break;
            }
           

        }


      
        //                switch (kv.Key)
        //                {
        //                    case "ContentId":
        //                        media.ContentId = kv.Value?.ToString();
        //                        break;
        //                    case "ContentUrl":
        //                        media.ContentUrl = kv.Value?.ToString();
        //                        break;
        //                    case "ContentType":
        //                        media.ContentType = kv.Value?.ToString();
        //                        break;
        //                    case "StreamType":
        //                        if (Enum.TryParse<StreamType>(kv.Value?.ToString(), true, out var streamType))
        //                        {
        //                            media.StreamType = streamType;
        //                        }
        //                        else
        //                        {
        //                            media.StreamType = StreamType.Buffered;
        //                        }
        //                        break;
        //                    case "Title":
        //                        media.Metadata = media.Metadata ?? new MediaMetadata();
        //                        media.Metadata.Title = kv.Value?.ToString();
        //                        break;
        //                    case "SubTitle":
        //                        media.Metadata = media.Metadata ?? new MediaMetadata();
        //                        media.Metadata.SubTitle = kv.Value?.ToString();
        //                        break;
        //                    case "Duration":
        //                        if (double.TryParse(kv.Value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var duration))
        //                        {
        //                            media.Duration = duration;
        //                        }
        //                        break;
        //                    default:
        //                        AnsiConsole.MarkupLine("[yellow]⚠️  Warning: Unknown key in configuration (playlist: " + playlist.Name + " ) item: [/]" + kv.Key);
        //                        break;
        //                }
       
        internal List<Media> GetMediaForId(string playlistId)
        {
            List<Media> result = new List<Media>();
            if (_Playlists.ContainsKey(playlistId))
            {
                result = _Playlists[playlistId];
            }
            return result;
        }
    }
}
