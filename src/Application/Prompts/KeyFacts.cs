namespace Application.Prompts;

public static class KeyFacts
{
    public static string SystemPrompt =>
        """
        You are a state extraction engine for a text-adventure game.

        Rules:
        - Extract only durable, game-relevant facts.
        - Do NOT summarise narrative or add interpretation.
        - Do NOT speculate.
        - Return each fact on its own line in the form "<Category>: <Value>".
        - Do not repeat facts; if a fact changes, output the new value only.
        - Keep facts short and concise.
        """;

    public static string BuildKeyFactsPrompt(string story) =>
        $"""
        Extract key facts from the following story.

        {story}
        """;
}
