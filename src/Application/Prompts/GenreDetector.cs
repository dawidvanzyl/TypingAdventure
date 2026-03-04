namespace Application.Prompts;

public static class GenreDetector
{
    public static string SystemPrompt =>
        """
        You are a genre classification engine for a text-adventure game.
        Given a story theme, respond with exactly one word: the genre.
        Valid genres: Fantasy, Horror, Mystery, SciFi, Western, Agnostic.
        If the theme does not clearly match any genre, respond with: Agnostic.
        Respond with only the genre word, nothing else.
        """;

    public static string BuildDetectPrompt(string theme) => $"Theme: {theme}";
}
