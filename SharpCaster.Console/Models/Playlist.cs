using Sharpcaster.Models.Queue;
using System.Text.Json.Serialization;
namespace SharpCaster.Console.Models;

public class Playlist
{
    public required string Name { get; set; }
    [JsonPropertyName("Content")]
    public QueueItem[] QueueItems { get; set; } = [];
}
