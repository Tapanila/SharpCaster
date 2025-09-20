using SharpCaster.Console.UI;
using System.Text.Json.Serialization;

namespace SharpCaster.Console.Models;

[JsonSerializable(typeof(CommandLineArgs))]
[JsonSerializable(typeof(ApplicationState))]
[JsonSerializable(typeof(Playlist))]
[JsonSerializable(typeof(Playlist[]))]
[JsonSerializable(typeof(UserSettingsModel))]
public partial class ConsoleJsonContext : JsonSerializerContext
{
}