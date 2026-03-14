namespace Application.Schemas;

public static class Western
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
		    "bounties": [{"name": "string", "reward": "string or null", "status": "string"}],
		    "outlaws": [{"name": "string", "gang": "string or null", "threat": "string", "status": "string"}]
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
		    "lawStatus": {
		      "playerReputation": "string or null",
		      "wantedLevel": "string or null",
		      "lawPresence": "string or null"
		    }
		  }
		}
		""";
}
