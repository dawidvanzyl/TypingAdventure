using Application.Helpers;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Helpers;

public class JsonCompressorTests
{
	[Fact]
	public void Minify_WithPrettyPrintedJson_RemovesWhitespace()
	{
		var pretty = """
			{
			  "dangerLevel": "low",
			  "setting": {
			    "currentLocation": "Forest"
			  }
			}
			""";

		var result = JsonCompressor.Minify(pretty);

		result.Should().Be("""{"dangerLevel":"low","setting":{"currentLocation":"Forest"}}""");
	}

	[Fact]
	public void Minify_WithAlreadyMinifiedJson_ReturnsSameContent()
	{
		var minified = """{"dangerLevel":"low","setting":{"currentLocation":"Forest"}}""";

		var result = JsonCompressor.Minify(minified);

		result.Should().Be(minified);
	}

	[Fact]
	public void Minify_WithInvalidJson_Throws()
	{
		var act = () => JsonCompressor.Minify("not valid json");

		act.Should().Throw<Exception>();
	}

	[Fact]
	public void Minify_WithNullOrEmptyInput_ReturnsInputUnchanged()
	{
		JsonCompressor.Minify("").Should().Be("");
		JsonCompressor.Minify("   ").Should().Be("   ");
	}

	[Fact]
	public void MinifyAndStrip_WithNullValues_RemovesNullFields()
	{
		var json = """{"dangerLevel":"low","setting":{"currentLocation":null,"exits":["north"]}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().NotContain("currentLocation");
		result.Should().Contain("exits");
		result.Should().Contain("north");
	}

	[Fact]
	public void MinifyAndStrip_WithEmptyArrays_RemovesEmptyArrayFields()
	{
		var json = """{"dangerLevel":"low","setting":{"currentLocation":"Hall","exits":[]}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().NotContain("exits");
		result.Should().Contain("currentLocation");
		result.Should().Contain("Hall");
	}

	[Fact]
	public void MinifyAndStrip_WithPopulatedArrays_PreservesArrayEntries()
	{
		var json = """{"dangerLevel":"high","characters":{"allies":["Gandalf","Frodo"],"enemies":[]}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().Contain("Gandalf");
		result.Should().Contain("Frodo");
		result.Should().NotContain("enemies");
	}

	[Fact]
	public void MinifyAndStrip_WithAllNullChildObject_RemovesParentObject()
	{
		var json = """{"dangerLevel":"low","setting":{"currentLocation":null,"locationDescription":null}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().NotContain("setting");
		result.Should().Contain("dangerLevel");
	}

	[Fact]
	public void MinifyAndStrip_WithInvalidJson_Throws()
	{
		var act = () => JsonCompressor.MinifyAndStrip("not valid json");

		act.Should().Throw<Exception>();
	}

	[Fact]
	public void MinifyAndStrip_WithNullOrEmptyInput_ReturnsInputUnchanged()
	{
		JsonCompressor.MinifyAndStrip("").Should().Be("");
		JsonCompressor.MinifyAndStrip("   ").Should().Be("   ");
	}

	[Fact]
	public void MinifyAndStrip_PreservesDangerLevel_WhenPresent()
	{
		var json = """{"dangerLevel":"critical","setting":{"currentLocation":null}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().Contain("\"dangerLevel\":\"critical\"");
	}

	[Fact]
	public void MinifyAndStrip_PreservesNestedDangerLevel_WhenPresent()
	{
		var json = """{"engine":{"dangerLevel":"critical"},"world":{"setting":{"currentLocation":null}}}""";

		var result = JsonCompressor.MinifyAndStrip(json);

		result.Should().Contain("\"dangerLevel\":\"critical\"");
		result.Should().NotContain("currentLocation");
	}

	[Fact]
	public void MinifyAndStrip_WithDuplicateKeys_Throws()
	{
		var act = () => JsonCompressor.MinifyAndStrip("{\"world\":{\"allies\":[\"Gandalf\"],\"allies\":[\"Frodo\"]}}");

		act.Should().Throw<Exception>();
	}
}
