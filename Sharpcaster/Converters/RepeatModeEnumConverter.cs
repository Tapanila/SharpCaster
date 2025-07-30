using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Converters
{
    public class RepeatModeEnumConverter : JsonConverter<RepeatModeType>
    {
        public override RepeatModeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumString = reader.GetString();
            switch (enumString)
            {
                case "REPEAT_OFF":
                    return RepeatModeType.OFF;
                case "REPEAT_ALL":
                    return RepeatModeType.ALL;
                case "REPEAT_SINGLE":
                    return RepeatModeType.SINGLE;
                case "REPEAT_ALL_AND_SHUFFLE":
                    return RepeatModeType.ALL_AND_SHUFFLE;
                default:
                    throw new JsonException($"Invalid value '{enumString}' for {nameof(RepeatModeType)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, RepeatModeType value, JsonSerializerOptions options)
        {
            string stringValue;
            switch (value)
            {
                case RepeatModeType.OFF:
                    stringValue = "REPEAT_OFF";
                    break;
                case RepeatModeType.ALL:
                    stringValue = "REPEAT_ALL";
                    break;
                case RepeatModeType.SINGLE:
                    stringValue = "REPEAT_SINGLE";
                    break;
                case RepeatModeType.ALL_AND_SHUFFLE:
                    stringValue = "REPEAT_ALL_AND_SHUFFLE";
                    break;
                default:
                    throw new JsonException($"Unsupported {nameof(RepeatModeType)} value: {value}");
            }

            writer?.WriteStringValue(stringValue);
        }
    }
}