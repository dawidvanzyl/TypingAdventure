namespace Application.Schemas;

public static class Horror
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
		    "threats": [{"name": "string", "type": "string", "status": "string", "description": "string"}],
		    "safeZones": [{"location": "string", "reason": "string"}]
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
		    "survivalStatus": {
		      "sanity": "string or null",
		      "health": "string or null"
		    }
		  }
		}
		""";
}
