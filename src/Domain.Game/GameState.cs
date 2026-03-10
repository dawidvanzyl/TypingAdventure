using Domain.Game.Enums;

namespace Domain.Game;

public class GameState
{
    public string Premise { get; set; } = "";
    public string StorySummary { get; set; } = "";
    public List<string> StoryLog { get; } = [];
    public string KeyFacts { get; set; } = "{}";
    public List<string> Inventory { get; } = [];
    public Genre DetectedGenre { get; set; } = Genre.Agnostic;

    public bool GameOver { get; set; } = false;
}
