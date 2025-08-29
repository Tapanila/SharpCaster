using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Converters
{
    public class StreamTypeEnumConverter : JsonConverter<StreamType>
    {
        public override StreamType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // Support numeric representations (0=None, 1=Live, 2=Buffered)
                if (reader.TryGetInt32(out var intVal))
                {
                    return intVal switch
                    {
                        0 => StreamType.None,
                        1 => StreamType.Live,
                        2 => StreamType.Buffered,
                        _ => throw new JsonException($"Invalid numeric value '{intVal}' for {nameof(StreamType)}"),
                    };
                }
                throw new JsonException($"Invalid numeric token for {nameof(StreamType)}");
            }

            // Default: string representations
            string? enumString = reader.GetString();
            return enumString switch
            {
                "NONE" => StreamType.None,
                "LIVE" => StreamType.Live,
                "BUFFERED" => StreamType.Buffered,
                _ => throw new JsonException($"Invalid value '{enumString}' for {nameof(StreamType)}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, StreamType value, JsonSerializerOptions options)
        {
            string stringValue = value switch
            {
                StreamType.None => "NONE",
                StreamType.Live => "LIVE",
                StreamType.Buffered => "BUFFERED",
                _ => throw new JsonException($"Unsupported {nameof(StreamType)} value: {value}"),
            };
            writer?.WriteStringValue(stringValue);
        }
    }
}
