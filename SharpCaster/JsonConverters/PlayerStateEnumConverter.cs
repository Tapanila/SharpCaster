using System;
using Newtonsoft.Json;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.JsonConverters
{
    public class PlayerStateEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var playerState = (PlayerState)value;
            switch (playerState)
            {
                case PlayerState.Buffering:
                    writer.WriteValue("BUFFERING");
                    break;
                case PlayerState.Idle:
                    writer.WriteValue("IDLE");
                    break;
                case PlayerState.Paused:
                    writer.WriteValue("PAUSED");
                    break;
                case PlayerState.Playing:
                    writer.WriteValue("PLAYING");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string) reader.Value;
            PlayerState? playerState = null;

            switch (enumString)
            {
                case "BUFFERING":
                    playerState = PlayerState.Buffering;
                    break;
                case "IDLE":
                    playerState = PlayerState.Idle;
                    break;
                case "PAUSED":
                    playerState = PlayerState.Paused;
                    break;
                case "PLAYING":
                    playerState = PlayerState.Playing;
                    break;
            }
            return playerState;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (string);
        }
    }
}