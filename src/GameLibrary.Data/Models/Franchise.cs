namespace GameLibrary.Data.Models;

public sealed class Franchise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
}
