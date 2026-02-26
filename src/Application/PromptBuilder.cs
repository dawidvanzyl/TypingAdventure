using Domain.Game;

namespace Application;

public static class PromptBuilder
{
    public static string NarratorSystemPrompt =>
"""
You are the narrator and game engine of a text-based typing adventure.

Rules:
- Write in second person.
- Do not decide actions for the player.
- Respect established facts.
- If an action is impossible, explain why and ask again.
- End every response by asking: "What do you do?"
- Keep responses under 300 words.
""";

    public static string BuildTurnPrompt(GameState state, string playerInput)
    {
        return $"""
Game premise:
{state.Premise}

Story summary:
{state.StorySummary}

Known facts:
{string.Join(", ", state.KeyFacts)}

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
