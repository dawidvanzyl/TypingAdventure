using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Helpers;

public static class JsonCompressor
{
	private static readonly JsonSerializerOptions _minifyOptions = new()
	{
		WriteIndented = false
	};

	public static string Minify(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return json;
		}

		using var document = JsonDocument.Parse(json);
		return JsonSerializer.Serialize(document.RootElement, _minifyOptions);
	}

	public static string MinifyAndStrip(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return json;
		}

		var node = JsonNode.Parse(json);
		if (node is null)
		{
			return json;
		}

		StripNullsAndEmpties(node);
		return node.ToJsonString(_minifyOptions);
	}

	private static void StripNullsAndEmpties(JsonNode node)
	{
		if (node is not JsonObject obj)
		{
			return;
		}

		var keysToRemove = new List<string>();

		foreach (var property in obj)
		{
			var value = property.Value;

			if (value is null)
			{
				keysToRemove.Add(property.Key);
				continue;
			}

			if (value is JsonValue jsonValue && jsonValue.GetValueKind() == JsonValueKind.Null)
			{
				keysToRemove.Add(property.Key);
				continue;
			}

			if (value is JsonArray array)
			{
				if (array.Count == 0)
				{
					keysToRemove.Add(property.Key);
				}
				else
				{
					foreach (var item in array)
					{
						StripNullsAndEmpties(item);
					}
				}

				continue;
			}

			if (value is JsonObject)
			{
				StripNullsAndEmpties(value);

				if (obj[property.Key] is JsonObject childObj && childObj.Count == 0)
				{
					keysToRemove.Add(property.Key);
				}
			}
		}

		foreach (var key in keysToRemove)
		{
			obj.Remove(key);
		}
	}
}
