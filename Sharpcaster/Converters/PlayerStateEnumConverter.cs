using Newtonsoft.Json;
using Sharpcaster.Models.Media;
using System;

namespace Sharpcaster.Converters
{
    public class PlayerStateEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var playerState = (PlayerStateType)value;
            switch (playerState)
            {
                case PlayerStateType.Buffering:
                    writer.WriteValue("BUFFERING");
                    break;
                case PlayerStateType.Idle:
                    writer.WriteValue("IDLE");
                    break;
                case PlayerStateType.Paused:
                    writer.WriteValue("PAUSED");
                    break;
                case PlayerStateType.Playing:
                    writer.WriteValue("PLAYING");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            PlayerStateType? playerState = null;

            switch (enumString)
            {
                case "BUFFERING":
                    playerState = PlayerStateType.Buffering;
                    break;
                case "IDLE":
                    playerState = PlayerStateType.Idle;
                    break;
                case "PAUSED":
                    playerState = PlayerStateType.Paused;
                    break;
                case "PLAYING":
                    playerState = PlayerStateType.Playing;
                    break;
            }
            return playerState;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}