namespace SharpCaster.Console.UI;

public class MenuNode
{
    public MenuNode(string name, string? id = null)
    {
        Name = name;
        Id = id ?? name;
    }
    public string Id { get; set; }
    public string Name { get; set; }
}
