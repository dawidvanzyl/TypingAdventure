namespace Application.Schemas;

public static class SciFi
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
		  "technology": {
		    "knownTech": ["string"],
		    "techItems": ["string"],
		    "systemsStatus": ["string"]
		  },
		  "factions": [{"name": "string", "relationship": "string", "description": "string"}],
		  "missionStatus": {
		    "primaryObjective": "string or null",
		    "secondaryObjectives": ["string"],
		    "completedObjectives": ["string"]
		  }
		}
		""";
}