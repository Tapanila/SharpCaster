namespace SharpCaster.Console.UI;

public class Category : MenuNode
{
    public Category(string name, string? id = null) : base(name, id) { }
    public List<MenuNode> Content { get; set; } = new List<MenuNode>();
}
