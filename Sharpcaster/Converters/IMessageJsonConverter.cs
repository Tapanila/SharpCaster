using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Media;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class IMessageJsonConverter : JsonConverter<IMessage>
{
    public override IMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Parse the JSON into a JsonDocument for inspection
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.TryGetProperty("type", out var typeElement))
            {
                var messageType = typeElement.GetString();

                // Map the type string to the appropriate runtime type.
                // Add more mappings as needed.
                Type runtimeType;
                if (messageType == "CONNECT")
                {
                    runtimeType = typeof(ConnectMessage);
                }
                else if (messageType == "MEDIA_STATUS")
                {
                    runtimeType = typeof(MediaStatusMessage);
                }
                else if (messageType == "LOAD_FAILED")
                {
                    runtimeType = typeof(LoadFailedMessage);
                }
                else
                {
                    runtimeType = typeof(Message);
                }

                // Deserialize into the determined runtime type
                return (IMessage)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), runtimeType, options);
            }
       

        // If no "type" property or can't determine type, fallback to a base type.
        return JsonSerializer.Deserialize<Message>(doc.RootElement.GetRawText(), options);
        }
    }

    public override void Write(Utf8JsonWriter writer, IMessage value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type of the value
        var runtimeType = value.GetType();
        JsonSerializer.Serialize(writer, value, runtimeType, options);
    }
}