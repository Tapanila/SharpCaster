using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Converters
{
    public class MediaCommandEnumConverter : JsonConverter<MediaCommand>
    {
        public override MediaCommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                int value = reader.GetInt32();
                // For flags enums, we don't use Enum.IsDefined since combined values are valid
                // Just cast the value directly - the flags enum will handle it correctly
                return (MediaCommand)value;
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string? enumString = reader.GetString();
                if (string.IsNullOrEmpty(enumString))
                {
                    throw new JsonException($"Invalid null or empty string for {nameof(MediaCommand)}");
                }

                if (Enum.TryParse<MediaCommand>(enumString, true, out MediaCommand result))
                {
                    return result;
                }
                throw new JsonException($"Invalid string value '{enumString}' for {nameof(MediaCommand)}");
            }

            throw new JsonException($"Unexpected token type '{reader.TokenType}' when reading {nameof(MediaCommand)}");
        }

        public override void Write(Utf8JsonWriter writer, MediaCommand value, JsonSerializerOptions options)
        {
            writer?.WriteNumberValue((int)value);
        }
    }
}