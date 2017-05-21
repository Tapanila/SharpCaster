using System;
using Newtonsoft.Json;
using SharpCaster.Models.Enums;

namespace SharpCaster.JsonConverters
{
    public class MetadataTypeEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var metadataType = (MetadataTypeEnum)value;
            switch (metadataType)
            {
                case MetadataTypeEnum.GENERIC:
                    writer.WriteValue(0);
                    break;
                case MetadataTypeEnum.MOVIE:
                    writer.WriteValue(1);
                    break;
                case MetadataTypeEnum.TV_SHOW:
                    writer.WriteValue(2);
                    break;
                case MetadataTypeEnum.MUSIC_TRACK:
                    writer.WriteValue(3);
                    break;
                case MetadataTypeEnum.PHOTO:
                    writer.WriteValue(4);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (Int64)reader.Value;
            MetadataTypeEnum metadataType;

            Enum.TryParse(enumString.ToString(), out metadataType);

            return metadataType;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }
    }
}
