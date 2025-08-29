using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Converters
{
    public class PlayerStateEnumConverter : JsonConverter<PlayerStateType>
    {
        public override PlayerStateType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? enumString = reader.GetString();
            return enumString switch
            {
                "BUFFERING" => PlayerStateType.Buffering,
                "IDLE" => PlayerStateType.Idle,
                "PAUSED" => PlayerStateType.Paused,
                "PLAYING" => PlayerStateType.Playing,
                "LOADING" => PlayerStateType.Loading,
                _ => throw new JsonException($"Invalid value '{enumString}' for {nameof(PlayerStateType)}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, PlayerStateType value, JsonSerializerOptions options)
        {
            string stringValue = value switch
            {
                PlayerStateType.Buffering => "BUFFERING",
                PlayerStateType.Idle => "IDLE",
                PlayerStateType.Paused => "PAUSED",
                PlayerStateType.Playing => "PLAYING",
                PlayerStateType.Loading => "LOADING",
                _ => throw new JsonException($"Unsupported {nameof(PlayerStateType)} value: {value}"),
            };
            writer?.WriteStringValue(stringValue);
        }
    }
}