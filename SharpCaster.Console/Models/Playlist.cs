using Sharpcaster.Models.Media;
using SharpCaster.Console.UI;

namespace SharpCaster.Console.Models;

public class Playlist : MenuNode
{
    public Playlist(string name, string? id = null) : base(name, id) { }
    
    public List<Media> Tracks { get; set; } = new List<Media>();
}
