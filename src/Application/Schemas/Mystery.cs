namespace Application.Schemas;

public static class Mystery
{
	public static string Schema =>
		"""
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
		  },
		  "clues": [{"description": "string", "location": "string", "relevance": "string"}],
		  "suspects": [{"name": "string", "motive": "string or null", "alibi": "string or null", "status": "string"}],
		  "revelations": ["string"]
		}
		""";
}