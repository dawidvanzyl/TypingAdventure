namespace Application.Schemas;

public static class Fantasy
{
	public static string Schema =>
		"""
		{
		  "world": {
		    "setting": {
		      "currentLocation": "string or null",
		      "locationDescription": "string or null",
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
		      "plotPoints": ["string"]
		    },
		    "quests": [{"name": "string", "status": "string", "description": "string"}],
		    "magicSystem": {
		      "knownSpells": ["string"],
		      "magicItems": ["string"]
		    },
		    "factions": [{"name": "string", "relationship": "string", "description": "string"}]
		  },
		  "engine": {
		    "dangerLevel": "low | medium | high | critical | fatal",
		    "setting": {
		      "discoveredLocations": ["string"]
		    },
		    "narrative": {
		      "warnings": ["string"]
		    },
		    "flags": {
		      "eventsTriggered": ["string"],
		      "knowledgeGained": ["string"]
		    },
		    "magicRules": ["string"]
		  }
		}
		""";
}
