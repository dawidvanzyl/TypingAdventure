using Application.Helpers;
using Domain.Game;

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

		Danger escalation rules:
		- dangerLevel starts at "low" and only ever increases unless the player earns relief through decisive action.
		- Escalate dangerLevel to "medium" if the player is passive, vague, or wastes a turn.
		- Escalate dangerLevel to "high" if the player continues to be passive, ignores warnings, or makes a reckless choice.
		- Escalate dangerLevel to "critical" if the player ignores repeated threats or takes a fatal action.
		- At "critical" dangerLevel the player is on the brink of death — make this viscerally clear in the narrative.
		- Passivity is fatal: doing nothing is never safe and always worsens the situation.
		- Never reduce dangerLevel simply because a turn passed; relief must be earned.
		""";

	public static string BuildTurnPrompt(GameState state, string playerInput)
	{
		return $"""
			Game premise:
			{state.Premise}

			Story summary:
			{state.StorySummary}

			Game state (JSON):
			{JsonCompressor.Minify(state.KeyFacts)}

			Inventory:
			{string.Join(", ", state.Inventory)}

			Last player action:
			{playerInput}

			Continue the story and ask what the player does next.
			""";
	}

	public static string BuildPremisePrompt(string theme) =>
		$"""
		Generate a unique {theme} premise in two paragraphs.
		Include setting, initial situation, and immediate tension.
		End by asking the player what they do first.
		""";

	public static string BuildSummaryPrompt(string fullStory) =>
		$"""
		Summarise the following story in 5–6 sentences.
		Preserve important events and facts.

		{fullStory}
		""";
}
