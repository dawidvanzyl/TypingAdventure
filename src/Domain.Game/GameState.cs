namespace Domain.Game;

public class GameState
{
    public string Premise { get; set; } = "";
    public string StorySummary { get; set; } = "";
    public List<string> StoryLog { get; } = [];
    public List<string> KeyFacts { get; } = [];
    public string KeyFactsJson { get; set; } = "{}";
	public List<string> Inventory { get; } = [];

    public bool GameOver { get; set; } = false;
}
