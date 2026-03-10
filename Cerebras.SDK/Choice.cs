using System.Text.Json.Serialization;

namespace Cerebras.SDK;

public class Choice
{
	[JsonPropertyName("message")]
	public Message Message { get; set; }
}
