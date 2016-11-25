using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCaster.Models.Enums;
using SharpCaster.Models.Metadata;

namespace SharpCaster.JsonConverters
{
    public class MetadataTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
         
            JObject jObject = JObject.Load(reader);
            
            var value = jObject.GetValue("metadataType").ToString();
            MetadataType metadataType;

            Enum.TryParse(value, out metadataType);
            switch (metadataType)
            {
                case MetadataType.GENERIC:
                    return jObject.ToObject<GenericMediaMetadata>();
                case MetadataType.MOVIE:
                    return jObject.ToObject<MovieMediaMetadata>();
                case MetadataType.TV_SHOW:
                    return jObject.ToObject<TvShowMediaMetadata>();
                case MetadataType.MUSIC_TRACK:
                    return jObject.ToObject<MusicTrackMediaMetadata>();
                case MetadataType.PHOTO:
                    return jObject.ToObject<PhotoMediaMetadata>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
