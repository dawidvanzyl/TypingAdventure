namespace Application.Schemas;

public static class Mystery
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
		    "suspects": [{"name": "string", "motive": "string or null", "alibi": "string or null", "status": "string"}]
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
		    "clues": [{"description": "string", "location": "string", "relevance": "string"}],
		    "revelations": ["string"]
		  }
		}
		""";
}
