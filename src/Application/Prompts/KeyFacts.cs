namespace Application.Prompts;

public static class KeyFacts
{
    public static string SystemPrompt =>
        """
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
        {
          "setting": {
            "currentLocation": "string or null",
            "locationDescription": "string or null",
            "discoveredLocations": ["string"],
            "exits": ["string"]
          },
          "characters": {
            "npcs": [{"name": "string", "description": "string", "status": "string"}],
            "allies": ["string"],
            "enemies": ["string"]
          },
          "objects": {
            "inventory": ["string"],
            "discovered": ["string"]
          },
          "narrative": {
            "atmosphere": "string or null",
            "timeProgress": "string or null",
            "objectives": ["string"],
            "warnings": ["string"],
            "plotPoints": ["string"]
          },
          "flags": {
            "eventsTriggered": ["string"],
            "knowledgeGained": ["string"]
          }
        }
        """;

    public static string BuildKeyFactsPrompt(string story) =>
        $"""
        Extract key facts from the following story and return ONLY valid JSON matching the schema.

        Story:
        {story}
        """;
}

