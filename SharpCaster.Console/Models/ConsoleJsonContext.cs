using System.Text.Json.Serialization;

namespace SharpCaster.Console.Models;

[JsonSerializable(typeof(CommandLineArgs))]
[JsonSerializable(typeof(ApplicationState))]
public partial class ConsoleJsonContext : JsonSerializerContext
{
}