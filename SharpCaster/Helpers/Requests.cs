using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;

namespace SharpCaster.Helpers
{
       public static class RequestIdProvider
        {
            public static int GetNext()
            {
                return Interlocked.Add(ref currentId, 1);
            }

            private static /*volatile*/ int currentId = new Random((int) DateTime.Now.Ticks).Next();
        }

        [DataContract]
        public abstract class Request
        {
            public Request(string requestType)
            {
                RequestType = requestType;

            }

            [DataMember(Name = "type")]
            public string RequestType { get; set; }


            public string ToJson()
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                return JsonConvert.SerializeObject(this, settings);
            }
        }

        [DataContract]
        public abstract class RequestWithId : Request
        {
            public RequestWithId(string requestType)
                : base(requestType)
            {
                RequestId = RequestIdProvider.GetNext();
            }

            [DataMember(Name = "requestId")]
            public int RequestId { get; set; }
        }


        [DataContract]
        public class LaunchRequest : RequestWithId
        {
            public LaunchRequest(string appId)
                : base("LAUNCH")
            {
                ApplicationId = appId;
            }

            [DataMember(Name = "appId")]
            public string ApplicationId { get; set; }
        }

        [DataContract]
        public class StopRequest : RequestWithId
        {
            public StopRequest(string sessionId)
                : base("STOP")
            {

            }

            [DataMember(Name = "sessionId")]
            public string SessionId { get; set; }
        }

        [DataContract]
        public class PlayRequest : RequestWithId
        {
            public PlayRequest()
                : base("PLAY")
            {
            }
        }

        [DataContract]
        public class PauseRequest : RequestWithId
        {
            public PauseRequest()
                : base("PAUSE")
            {
            }
        }

        [DataContract]
        public class GetStatusRequest : RequestWithId
        {
            public GetStatusRequest()
                : base("GET_STATUS")
            {
            }
        }

        [DataContract]
        public class PingRequest : Request
        {
            public PingRequest()
                : base("PING")
            {
            }
        }

        [DataContract]
        public class PongRequest : Request
        {
            public PongRequest()
                : base("PONG")
            {
            }
        }

        [DataContract]
        public class ConnectRequest : Request
        {
            public ConnectRequest()
                : base("CONNECT")
            {
            }
        }

        [DataContract]
        public class CloseRequest : Request
        {
            public CloseRequest()
                : base("CLOSE")
            {
            }
        }

        [DataContract]
        public class GetAppAvailabilityRequest : RequestWithId
        {
            public GetAppAvailabilityRequest(string[] appIds)
                : base("GET_APP_AVAILABILITY")
            {

            }

            [DataMember(Name = "appId")]
            public string[] ApplicationId { get; set; }
        }

        [DataContract]
        public class Metadata
        {
            public Metadata(int metadataType = 0)
            {
                MetadataType = metadataType;
            }

            [DataMember(Name = "metadataType")]
            public int MetadataType { get; set; }
        }

        [DataContract]
        public class GenericMetadata : Metadata
        {
            public GenericMetadata(string title, string subtitle = "", IList<string> images = null)
                : base()
            {
                Title = title;
                Subtitle = subtitle;
                _Artworks = images;
            }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "subtitle")]
            public string Subtitle { get; set; }

            [DataMember(Name = "images")]
            public List<Artwork> Artworks
            {
                get
                {
                    var aw = new List<Artwork>();
                    for (var i = 0; i < _Artworks.Count; i++)
                        aw.Add(new Artwork()
                        {
                            url = _Artworks[i]
                        });
                    return aw;
                }
            }

            private IList<string> _Artworks { get; }
        }

        [DataContract]
        public class MovieMetadata : Metadata
        {
            public MovieMetadata(string title, DateTime releaseDate, IList<string> images)
                : base(1)
            {
                Title = title;
                ReleaseDate = releaseDate;
                _Artworks = images;
            }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "releaseDate")]
            public DateTime ReleaseDate { get; set; }

            [DataMember(Name = "images")]
            public List<Artwork> Artworks
            {
                get
                {
                    var aw = new List<Artwork>();
                    for (var i = 0; i < _Artworks.Count; i++)
                        aw.Add(new Artwork()
                        {
                            url = _Artworks[i]
                        });
                    return aw;
                }
            }

            private IList<string> _Artworks { get; }
        }

        [DataContract]
        public class TVShowMetadata : Metadata
        {
            public TVShowMetadata(int episode,
                int season,
                string episodeName,
                string seriesName,
                DateTime airdate,
                IList<string> images = null)
                : base(2)
            {
                Episode = episode;
                Season = season;
                EpisodeName = episodeName;
                SeriesName = seriesName;
                Airdate = airdate;
                _Artworks = images;
            }

            [DataMember(Name = "episode")]
            public int Episode { get; set; }

            [DataMember(Name = "season")]
            public int Season { get; set; }

            [DataMember(Name = "title")]
            public string EpisodeName { get; set; }

            [DataMember(Name = "seriesTitle")]
            public string SeriesName { get; set; }

            [DataMember(Name = "originalAirdate")]
            public DateTime Airdate { get; set; }

            [DataMember(Name = "images")]
            public List<Artwork> Artworks
            {
                get
                {
                    var aw = new List<Artwork>();
                    for (var i = 0; i < _Artworks.Count; i++)
                        aw.Add(new Artwork()
                        {
                            url = _Artworks[i]
                        });
                    return aw;
                }
            }

            private IList<string> _Artworks { get; }
        }

        [DataContract]
        public class PhotoMetadata : Metadata
        {
            public PhotoMetadata(int width,
                int height,
                string title = "")
                : base(4)
            {
                Width = width;
                Height = height;

                if (title == "")
                    Title = null;
                else
                    Title = title;
            }

            [DataMember(Name = "width")]
            public int Width { get; set; }

            [DataMember(Name = "height")]
            public int Height { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }
        }

        [DataContract]
        public class MusicTrackMetadata : Metadata
        {
            public MusicTrackMetadata(string trackName, string artist, IList<String> imageUrls)
                : base(3)
            {
                TrackName = trackName;
                Artist = artist;
                _Artworks = imageUrls;
            }

            [DataMember(Name = "title")]
            public string TrackName { get; set; }

            [DataMember(Name = "artist")]
            public string Artist { get; set; }

            [DataMember(Name = "images")]
            public List<Artwork> Artworks
            {
                get
                {
                    var aw = new List<Artwork>();
                    for (var i = 0; i < _Artworks.Count; i++)
                        aw.Add(new Artwork()
                        {
                            url = _Artworks[i]
                        });
                    return aw;
                }
            }

            private IList<string> _Artworks { get; }
        }

        public class Artwork
        {
            [DataMember(Name = "url")]
            public string url { get; set; }
        }

    public class MagineUrl
    {
        public string channelId { get; set; }
        public string programId { get; set; }
    }

        [DataContract]
        public class Media
        {
            public Media(string url, string contentType, Metadata metadata = null, string streamType = "BUFFERED", double duration = 0d, object customData = null)
            {
                Url = url;
                ContentType = contentType;
                StreamType = streamType;
                Duration = duration;
                Metadata = metadata;
                CustomData = customData;
            }

            [DataMember(Name = "contentId")]
            public string Url { get; set; }

            [DataMember(Name = "contentType")]
            public string ContentType { get; set; }

            [DataMember(Name = "metadata")]
            public Metadata Metadata { get; set; }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>
            ///     Gets or sets the type of the stream. This can be BUFFERED, LIVE or NONE
            /// </summary>
            ///
            /// <value>
            ///     The type of the stream.
            /// </value>
            ///-------------------------------------------------------------------------------------------------
            [DataMember(Name = "streamType")]
            public string StreamType { get; set; }

            [DataMember(Name = "duration")]
            public double Duration { get; set; }

            [DataMember(Name = "customData")]
            public object CustomData { get; set; }
        }

        [DataContract]
        public class LoadRequest : RequestWithId
        {
            public LoadRequest(string sessionId, Media media, bool autoPlay, double currentTime,
                object customData = null)
                : base("LOAD")
            {
                SessionId = sessionId;
                Media = media;
                AutoPlay = autoPlay;
                CurrentTime = currentTime;
                Customdata = customData;

                //if (Customdata == null)
                //    Customdata = new Dictionary<string, string>();
            }

            [DataMember(Name = "sessionId")]
            public string SessionId { get; private set; }

            [DataMember(Name = "media")]
            public Media Media { get; private set; }

            [DataMember(Name = "autoplay")]
            public bool AutoPlay { get; private set; }

            [DataMember(Name = "currentTime")]
            public double CurrentTime { get; private set; }

            [DataMember(Name = "customData")]
            public object Customdata { get; }
        }
    }


