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
            string? enumString = reader.GetString();
            return enumString switch
            {
                "REPEAT_OFF" => RepeatModeType.OFF,
                "REPEAT_ALL" => RepeatModeType.ALL,
                "REPEAT_SINGLE" => RepeatModeType.SINGLE,
                "REPEAT_ALL_AND_SHUFFLE" => RepeatModeType.ALL_AND_SHUFFLE,
                _ => throw new JsonException($"Invalid value '{enumString}' for {nameof(RepeatModeType)}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, RepeatModeType value, JsonSerializerOptions options)
        {
            string stringValue = value switch
            {
                RepeatModeType.OFF => "REPEAT_OFF",
                RepeatModeType.ALL => "REPEAT_ALL",
                RepeatModeType.SINGLE => "REPEAT_SINGLE",
                RepeatModeType.ALL_AND_SHUFFLE => "REPEAT_ALL_AND_SHUFFLE",
                _ => throw new JsonException($"Unsupported {nameof(RepeatModeType)} value: {value}"),
            };
            writer?.WriteStringValue(stringValue);
        }
    }
}