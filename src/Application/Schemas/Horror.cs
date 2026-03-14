namespace Application.Schemas;

public static class Horror
{
	public static string Schema =>
		"""
		{
		  "dangerLevel": "low | medium | high | critical",
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
		  "threats": [{"name": "string", "type": "string", "status": "string", "description": "string"}],
		  "safeZones": [{"location": "string", "reason": "string"}],
		  "survivalStatus": {
		    "sanity": "string or null",
		    "health": "string or null",
		    "resources": ["string"]
		  }
		}
		""";
}