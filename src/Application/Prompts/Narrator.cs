using Application.Helpers;
using Domain.Game;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Prompts;

public static class Narrator
{
	public static string SystemPrompt =>
		"""
		You are the narrator and game engine of a text-based typing adventure.

		Rules:
		- Write in second person.
		- Do not decide actions for the player.
		- Respect established facts.
		- If an action is impossible, explain why and ask again.
		- End every response by asking: "What do you do?"
		- Keep responses under 300 words.

		World and engine state rules:
		- The world state contains facts you write from — setting, characters, objects, atmosphere.
		- The engine state contains hidden mechanics that shape the story but must NEVER be mentioned by name in the narrative.
		- Never say "the danger level is high" or reference any engine field directly.
		- Translate engine.dangerLevel into felt, embodied experience: tension, physical sensation, environmental dread.

		Danger escalation rules:
		- dangerLevel starts at "low" and only ever increases unless the player earns relief through decisive action.
		- Escalate dangerLevel to "medium" if the player is passive, vague, or wastes a turn.
		- Escalate dangerLevel to "high" if the player continues to be passive, ignores warnings, or makes a reckless choice.
		- Escalate dangerLevel to "critical" if the player ignores repeated threats or takes a near-fatal action — they are on the brink, make this viscerally clear.
		- Escalate dangerLevel to "fatal" only when the player is already at "critical" and takes a final, irreversible action. Write the death or end scene fully.
		- When dangerLevel is "fatal", write the death scene and do NOT end with "What do you do?"
		- Passivity is fatal: doing nothing is never safe and always worsens the situation.
		- Never reduce dangerLevel simply because a turn passed; relief must be earned.
		""";

	public static string BuildTurnPrompt(GameState state, string playerInput) =>
		$"""
		Game premise:
		{state.Premise}

		Story summary:
		{state.StorySummary}

		{BuildStateBlock(state)}

		Inventory:
		{string.Join(", ", state.Inventory)}

		Last player action:
		{playerInput}

		Continue the story and ask what the player does next.
		""";

	public static string BuildFinalTurnPrompt(GameState state) =>
		$"""
		Game premise:
		{state.Premise}

		Story summary:
		{state.StorySummary}

		{BuildStateBlock(state)}

		Inventory:
		{string.Join(", ", state.Inventory)}

		The player's fate is sealed. Write the final scene. Do NOT ask "What do you do?" — the story is over.
		""";

	public static string BuildPremisePrompt(string theme) =>
		$"""
		Generate a unique {theme} premise in two paragraphs.
		Include setting, initial situation, and immediate tension.
		End with the question: What do you do?
		""";

	public static string BuildSummaryPrompt(string fullStory) =>
		$"""
		Summarise the following story in 5–6 sentences.
		Preserve important events and facts.

		{fullStory}
		""";

	private static string BuildStateBlock(GameState state)
	{
		try
		{
			var root = JsonNode.Parse(state.KeyFacts);
			var worldNode = root?["world"];
			var engineNode = root?["engine"];

			if (worldNode != null && engineNode != null)
			{
				var worldBlock = JsonCompressor.MinifyAndStrip(worldNode.ToJsonString());
				var engineBlock = JsonCompressor.MinifyAndStrip(engineNode.ToJsonString());

				return $"""
					World state (write from this):
					{worldBlock}

					Engine state (obey silently, never mention by name):
					{engineBlock}
					""";
			}
		}
		catch (JsonException)
		{
			// fall through to single-block fallback
		}

		return $"""
			Game state (JSON):
			{JsonCompressor.MinifyAndStrip(state.KeyFacts)}
			""";
	}
}
