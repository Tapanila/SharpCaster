using Microsoft.Extensions.Configuration;
using Sharpcaster.Models.Media;
using SharpCaster.Console.Controllers;
using SharpCaster.Console.Models;
using SharpCaster.Console.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Console.Services
{
    public class PlaylistService
    {
        private Node _playlistTree;
        private Dictionary<string, List<Media>> _Ids = new Dictionary<string, List<Media>>();

        public PlaylistService(ApplicationState state, DeviceService deviceService, IConfiguration config)
        {
            _playlistTree = new Node("root");
            var _playlists = config.GetSection("Playlists");

            AddPlaylists(_playlistTree, _playlists);
        }

        public bool HasContent()
        {
            return _playlistTree != null;
        }

        public Node GetRoot()
        {
            return _playlistTree;
        }
        public bool IsPlaylistId(string arg)
        {
            return _Ids.Keys.Contains(arg);
        }

        private void AddPlaylists(Node parent, IConfigurationSection playlists)
        {
            List<Node> children = new List<Node>();
            var subsections = playlists.GetChildren();

            foreach (var section in subsections)
            {
                var data = section.GetChildren().FirstOrDefault();
                if (data != null && data.Key.Equals("0"))
                {
                    // This section only contains numbered items -> array of objects(Media)
                    var playlist = new Node(section.Key);
                    List<Media> tracks = new List<Media>();

                    // This works also but it is not easy to add meaningfull warnings if something is typed wrong in application.json
                    //List<Media>? tracks = mediaArray.Select(configSection =>
                    //        new Media()
                    //        {
                    //            ContentUrl = configSection["ContentUrl"]!.ToString(),
                    //            Metadata = new MediaMetadata() { Title = configSection["Title"]!.ToString() }
                    //        })?.ToList();

                    var mediaArray = section.GetChildren();
                    var e = mediaArray.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var media = new Media();
                        var keyValues = e.Current.GetChildren().AsEnumerable();
                        foreach (var kv in keyValues)
                        {
                            switch (kv.Key)
                            {
                                case "ContentId":
                                    media.ContentId = kv.Value?.ToString();
                                    break;
                                case "ContentUrl":
                                    media.ContentUrl = kv.Value?.ToString();
                                    break;
                                case "ContentType":
                                    media.ContentType = kv.Value?.ToString();
                                    break;
                                case "StreamType":
                                    if (Enum.TryParse<StreamType>(kv.Value?.ToString(), true, out var streamType))
                                    {
                                        media.StreamType = streamType;
                                    }
                                    else
                                    {
                                        media.StreamType = StreamType.Buffered;
                                    }
                                    break;
                                case "Title":
                                    media.Metadata = media.Metadata ?? new MediaMetadata();
                                    media.Metadata.Title = kv.Value?.ToString();
                                    break;
                                case "SubTitle":
                                    media.Metadata = media.Metadata ?? new MediaMetadata();
                                    media.Metadata.SubTitle = kv.Value?.ToString();
                                    break;
                                case "Duration":
                                    if (double.TryParse(kv.Value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var duration))
                                    {
                                        media.Duration = duration;
                                    }
                                    break;
                                default:
                                    AnsiConsole.MarkupLine("[yellow]⚠️  Warning: Unknown key in configuration (playlist: " + playlist.Name + " ) item: [/]" + kv.Key);
                                    break;
                            }
                        }
                        if (media.ContentUrl == null && media.ContentId == null)
                        {
                            AnsiConsole.MarkupLine("[yellow]⚠️  Warning: Skipping media item with no ContentUrl or ContentId in playlist: [/]" + playlist.Name);
                        }
                        else
                        {
                            tracks.Add(media);
                            if (media.ContentId != null)
                            {
                                if (_Ids.ContainsKey(media.ContentId))
                                {
                                    _Ids[media.ContentId].Add(media);
                                }
                                else
                                {
                                    _Ids[media.ContentId] = new List<Media>() { media };
                                }
                            }
                        }
                    }

                    //List<Media>? t = mediaArray.Select(configSection =>
                    //        new Media()
                    //        {
                    //            ContentUrl = configSection["ContentUrl"]!.ToString(),
                    //            Metadata = new MediaMetadata() { Title = configSection["Title"]!.ToString() }
                    //        })?.ToList();
                    playlist.Data = tracks;
                    children.Add(playlist);
                }
                else
                {
                    // This section contains subsections -> create a new node and recurse
                    var container = new Node(section.Key);
                    children.Add(container);
                    AddPlaylists(container, section);
                }
            }
            parent.Data = children;
        }

        internal List<Media> GetMediaForId(string playlistId)
        {
            List<Media> result = new List<Media>();
            if (_Ids.ContainsKey(playlistId))
            {
                result = _Ids[playlistId];
            }
            return result;
        }
    }
}
