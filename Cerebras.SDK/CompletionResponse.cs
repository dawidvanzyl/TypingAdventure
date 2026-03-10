using System.Text.Json.Serialization;

namespace Cerebras.SDK;

public class CompletionResponse
{
	[JsonPropertyName("choices")]
	public Choice[] Choices { get; set; }
}
