using Sharpcaster.Test.customChannel;
using System.Text.Json.Serialization;

namespace Sharpcaster.Test.customMessage
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
    [JsonSerializable(typeof(WebMessage))]
    
    public partial class WebMessageSerializationContext : JsonSerializerContext
    {
    }
}
