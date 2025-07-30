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
            string enumString = reader.GetString();
            switch (enumString)
            {
                case "BUFFERING":
                    return PlayerStateType.Buffering;
                case "IDLE":
                    return PlayerStateType.Idle;
                case "PAUSED":
                    return PlayerStateType.Paused;
                case "PLAYING":
                    return PlayerStateType.Playing;
                case "LOADING":
                    return PlayerStateType.Loading;

                default:
                    throw new JsonException($"Invalid value '{enumString}' for {nameof(PlayerStateType)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, PlayerStateType value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case PlayerStateType.Buffering:
                    writer?.WriteStringValue("BUFFERING");
                    break;
                case PlayerStateType.Idle:
                    writer?.WriteStringValue("IDLE");
                    break;
                case PlayerStateType.Paused:
                    writer?.WriteStringValue("PAUSED");
                    break;
                case PlayerStateType.Playing:
                    writer?.WriteStringValue("PLAYING");
                    break;
                case PlayerStateType.Loading:
                    writer?.WriteStringValue("LOADING");
                    break;
                default:
                    throw new JsonException($"Unsupported {nameof(PlayerStateType)} value: {value}");
            }
        }
    }
}