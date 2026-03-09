using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.AI.Tests.Helpers;

public class MockConfiguration : IConfiguration
{
	private readonly Dictionary<string, string> _values = new()
	{
		{ "AiClient:ApiKey", "test-key" },
		{ "AiClient:EndPoint", "https://api.test.com" },
		{ "AiClient:Model", "test-model" },
		{ "AiClient:MaxTokens", "100" },
		{ "AiClient:Temperature", "0.7" }
	};

	public string this[string key]
	{
		get => _values.TryGetValue(key, out var value) ? value : null;
		set => _values[key] = value;
	}

	public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();

	public IChangeToken GetReloadToken() => new NoChangeToken();

	public IConfigurationSection GetSection(string key) => new MockConfigurationSection(key, this);
}
