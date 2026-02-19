namespace TypingAdventure;

public class GameState
{
    public string Premise { get; set; } = "";
    public string StorySummary { get; set; } = "";
    public List<string> StoryLog { get; } = new();
    public List<string> KeyFacts { get; } = new();
    public List<string> Inventory { get; } = new();

    public bool GameOver { get; set; } = false;
}
