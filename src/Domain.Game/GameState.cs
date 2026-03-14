using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Game.Enums;

namespace Domain.Game;

public class GameState
{
	public string Premise { get; set; } = "";
	public string StorySummary { get; set; } = "";
	public List<string> StoryLog { get; } = [];
	public string KeyFacts { get; private set; } = "{}";
	public List<string> Inventory { get; } = [];
	public Genre DetectedGenre { get; set; } = Genre.Agnostic;
	public bool GameOver { get; set; } = false;

	public void ApplyKeyFacts(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return;
		}

		try
		{
			ValidateNoDuplicateKeys(JsonNode.Parse(json));

			using var document = JsonDocument.Parse(json);

			KeyFacts = json;

			if (document.RootElement.TryGetProperty("engine", out var engineElement) &&
				engineElement.TryGetProperty("dangerLevel", out var dangerLevelElement) &&
				dangerLevelElement.GetString()?.Equals("fatal", StringComparison.OrdinalIgnoreCase) == true)
			{
				GameOver = true;
			}
		}
		catch (JsonException)
		{
			// retain previous KeyFacts
		}
		catch (ArgumentException)
		{
			// retain previous KeyFacts — duplicate keys in AI response
		}
	}

	/// <summary>
	/// Traverses the entire JSON tree to force <see cref="JsonObject"/> to initialise its
	/// internal dictionary for each node. <see cref="JsonNode.Parse"/> is lenient and accepts
	/// duplicate keys silently; the <c>ArgumentException</c> is only thrown lazily, when the
	/// dictionary is first built during enumeration. Calling this before assigning
	/// <see cref="KeyFacts"/> ensures duplicate-key responses from the AI are caught and
	/// rejected early.
	/// </summary>
	private static void ValidateNoDuplicateKeys(JsonNode node)
	{
		if (node is not JsonObject obj)
		{
			return;
		}

		foreach (var property in obj)
		{
			ValidateNoDuplicateKeys(property.Value);
		}
	}
}
