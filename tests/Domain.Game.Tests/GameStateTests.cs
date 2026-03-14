using Domain.Game;
using FluentAssertions;
using Xunit;

namespace Domain.Game.Tests;

public class GameStateTests
{
	[Fact]
	public void DefaultState_HasExpectedDefaults()
	{
		var state = new GameState();

		state.Premise.Should().BeEmpty();
		state.StorySummary.Should().BeEmpty();
		state.GameOver.Should().BeFalse();
		state.StoryLog.Should().BeEmpty();
		state.KeyFacts.Should().Be("{}");
		state.Inventory.Should().BeEmpty();
	}

	[Fact]
	public void ApplyKeyFacts_WithValidJson_UpdatesKeyFacts()
	{
		var state = new GameState();
		var json = """{"world":{"setting":{"currentLocation":"Forest"}},"engine":{"dangerLevel":"low"}}""";

		state.ApplyKeyFacts(json);

		state.KeyFacts.Should().Be(json);
	}

	[Fact]
	public void ApplyKeyFacts_WhenDangerLevelIsFatal_SetsGameOver()
	{
		var state = new GameState();
		var json = """{"world":{"setting":{"currentLocation":"Abyss"}},"engine":{"dangerLevel":"fatal"}}""";

		state.ApplyKeyFacts(json);

		state.GameOver.Should().BeTrue();
	}

	[Fact]
	public void ApplyKeyFacts_WhenDangerLevelIsCritical_DoesNotSetGameOver()
	{
		var state = new GameState();
		var json = """{"world":{"setting":{"currentLocation":"Abyss"}},"engine":{"dangerLevel":"critical"}}""";

		state.ApplyKeyFacts(json);

		state.GameOver.Should().BeFalse();
	}

	[Fact]
	public void ApplyKeyFacts_WhenJsonIsInvalid_RetainsPreviousKeyFacts()
	{
		var state = new GameState();
		var original = """{"world":{"setting":{"currentLocation":"Hall"}},"engine":{"dangerLevel":"low"}}""";
		state.ApplyKeyFacts(original);

		state.ApplyKeyFacts("not valid json at all");

		state.KeyFacts.Should().Be(original);
		state.GameOver.Should().BeFalse();
	}

	[Fact]
	public void ApplyKeyFacts_WithDuplicateKeys_RetainsPreviousKeyFacts()
	{
		var state = new GameState();
		var original = """{"world":{"setting":{"currentLocation":"Hall"}},"engine":{"dangerLevel":"low"}}""";
		state.ApplyKeyFacts(original);

		state.ApplyKeyFacts("{\"world\":{\"allies\":[\"Gandalf\"],\"allies\":[\"Frodo\"]}}");

		state.KeyFacts.Should().Be(original);
		state.GameOver.Should().BeFalse();
	}

	[Fact]
	public void ApplyKeyFacts_WithDeeplyNestedDuplicateKeys_RetainsPreviousKeyFacts()
	{
		var state = new GameState();
		var original = """{"world":{"setting":{"currentLocation":"Hall"}},"engine":{"dangerLevel":"low"}}""";
		state.ApplyKeyFacts(original);

		state.ApplyKeyFacts("{\"world\":{\"setting\":{\"location\":\"Hall\",\"location\":\"Tower\"}}}");

		state.KeyFacts.Should().Be(original);
		state.GameOver.Should().BeFalse();
	}
}
