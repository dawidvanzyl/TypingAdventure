using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.AI.Tests.Helpers;

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
