using System;
using Newtonsoft.Json;
using SharpCaster.Models.Enums;

namespace SharpCaster.JsonConverters
{
    public class MediaErrorReasonEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var metadataType = (MediaErrorReasonEnum)value;
            writer.WriteValue(metadataType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            MediaErrorReasonEnum metadataType;

            Enum.TryParse(enumString, out metadataType);

            return metadataType;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
