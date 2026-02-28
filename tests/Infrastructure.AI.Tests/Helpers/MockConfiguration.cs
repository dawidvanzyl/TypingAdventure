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

public class MockConfigurationSection : IConfigurationSection
{
	private readonly string _key;
	private readonly IConfiguration _configuration;

	public MockConfigurationSection(string key, IConfiguration configuration)
	{
		_key = key;
		_configuration = configuration;
	}

	public string Key => _key;
	public string Path => _key;

	public string Value
	{
		get => _configuration[_key];
		set => _configuration[_key] = value;
	}

	public string this[string key]
	{
		get => _configuration[$"{_key}:{key}"];
		set => _configuration[$"{_key}:{key}"] = value;
	}

	public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();

	public IChangeToken GetReloadToken() => new NoChangeToken();

	public IConfigurationSection GetSection(string key) => new MockConfigurationSection($"{_key}:{key}", _configuration);
}

public class NoChangeToken : IChangeToken
{
	public bool HasChanged => false;
	public bool ActiveChangeCallbacks => false;
	public IDisposable RegisterChangeCallback(Action<object> callback, object state) => throw new NotImplementedException();
}
