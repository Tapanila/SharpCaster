using System;
using Newtonsoft.Json;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.JsonConverters
{
    public class StreamTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var streamType = (StreamType)value;
            switch (streamType)
            {
                case StreamType.BUFFERED:
                    writer.WriteValue("BUFFERED");
                    break;
                case StreamType.LIVE:
                    writer.WriteValue("LIVE");
                    break;
                case StreamType.NONE:
                    writer.WriteValue("NONE");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            StreamType streamType;

            Enum.TryParse(enumString, out streamType);

            return streamType;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
