namespace GameLibrary.Data.Models;

public sealed class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int FranchiseId { get; set; }
    public int PlatformId { get; set; }
}
