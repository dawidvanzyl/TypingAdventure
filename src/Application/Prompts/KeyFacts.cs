namespace Application.Prompts;

public static class KeyFacts
{
    public static string BuildSystemPrompt(string jsonSchema) =>
        $"""
        You are a state extraction engine for a text-adventure game.

        Rules:
        - Extract only durable, game-relevant facts into structured JSON.
        - Do NOT summarise narrative or add interpretation.
        - Do NOT speculate.
        - Return ONLY valid JSON (no additional text).
        - Include all categories from the schema, even if empty.
        - For arrays, provide values that have appeared in the story.
        - For null fields, use null if information is unknown.
        - When a fact changes, update the value (do not keep old values).
        - Keep values short and concise.

        Expected JSON Schema:
        {jsonSchema}
        """;

    public static string BuildKeyFactsPrompt(string story) =>
        $"""
        Extract key facts from the following story and return ONLY valid JSON matching the schema.

        Story:
        {story}
        """;

    public static string BuildUpdatePrompt(string currentJson, string latestTurn) =>
        $"""
        Update the existing key facts JSON by incorporating the new story content below.

        Rules:
        - Return the full updated JSON, not a diff.
        - Preserve all existing facts that are not contradicted by the new content.
        - Update fields where facts have changed (e.g. current location, character status).
        - Add new facts discovered in the new content.
        - Do NOT remove array entries unless the story explicitly contradicts them.
        - Return ONLY valid JSON (no additional text).

        Current key facts:
        {currentJson}

        New story content:
        {latestTurn}
        """;
}

