using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Converters
{
    public class MediaMetadataJsonConverter : JsonConverter<MediaMetadata>
    {
        public override MediaMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            MetadataType? metadataType = null;
            if (root.TryGetProperty("metadataType", out var mtProp))
            {
                if (mtProp.ValueKind == JsonValueKind.Number && mtProp.TryGetInt32(out var mt))
                {
                    metadataType = (MetadataType)mt;
                }
                else if (mtProp.ValueKind == JsonValueKind.String)
                {
                    var mtStr = mtProp.GetString();
                    if (Enum.TryParse<MetadataType>(mtStr, ignoreCase: true, out var parsed))
                        metadataType = parsed;
                }
            }

            Type targetType = metadataType switch
            {
                MetadataType.Music => typeof(MusicTrackMetadata),
                MetadataType.Movie => typeof(MovieMetadata),
                MetadataType.TVShow => typeof(TVShowMetadata),
                MetadataType.Photo => typeof(PhotoMetadata),
                MetadataType.AudiobookChapter => typeof(AudiobookChapterMetadata),
                _ => typeof(MediaMetadata)
            };

            var json = root.GetRawText();
            var typeInfo = options?.GetTypeInfo(targetType) ?? throw new JsonException($"No JsonTypeInfo registered for {targetType.FullName}");
            var result = JsonSerializer.Deserialize(json, typeInfo);
            return (MediaMetadata)(result ?? throw new JsonException($"Failed to deserialize {targetType.Name}"));
        }

        public override void Write(Utf8JsonWriter writer, MediaMetadata value, JsonSerializerOptions options)
        {
            // Serialize using the runtime type for correct shape without reflection-based overloads
            var runtimeType = (value?.GetType()) ?? throw new JsonException("Cannot serialize null MediaMetadata");
            var typeInfo = (options?.GetTypeInfo(runtimeType)) ?? throw new JsonException($"No JsonTypeInfo registered for {runtimeType.FullName}");
            JsonSerializer.Serialize(writer, value, typeInfo);
        }
    }
}
