using Newtonsoft.Json;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpcaster.Converters
{
    public class RepeatModeEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var repeatModeType = (RepeatModeType)value;
            switch (repeatModeType)
            {
                case RepeatModeType.OFF:
                    writer.WriteValue("REPEAT_OFF");
                    break;
                case RepeatModeType.ALL:
                    writer.WriteValue("REPEAT_ALL");
                    break;
                case RepeatModeType.SINGLE:
                    writer.WriteValue("REPEAT_SINGLE");
                    break;
                case RepeatModeType.ALL_AND_SHUFFLE:
                    writer.WriteValue("REPEAT_ALL_AND_SHUFFLE");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            RepeatModeType? repeatModeType = null;

            switch (enumString)
            {
                case "REPEAT_OFF":
                    repeatModeType = RepeatModeType.OFF;
                    break;
                case "REPEAT_ALL":
                    repeatModeType = RepeatModeType.ALL;
                    break;
                case "REPEAT_SINGLE":
                    repeatModeType = RepeatModeType.SINGLE;
                    break;
                case "REPEAT_ALL_AND_SHUFFLE":
                    repeatModeType = RepeatModeType.ALL_AND_SHUFFLE;
                    break;
            }
            return repeatModeType;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
