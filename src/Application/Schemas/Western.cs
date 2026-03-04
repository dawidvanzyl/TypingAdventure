namespace Application.Schemas;

public static class Western
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
		  "bounties": [{"name": "string", "reward": "string or null", "status": "string"}],
		  "outlaws": [{"name": "string", "gang": "string or null", "threat": "string", "status": "string"}],
		  "lawStatus": {
		    "playerReputation": "string or null",
		    "wantedLevel": "string or null",
		    "lawPresence": "string or null"
		  }
		}
		""";
}