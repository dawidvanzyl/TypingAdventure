using Application.Prompts;
using Domain.Game;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class NarratorTests
{
	[Fact]
	public void BuildTurnPrompt_WhenKeyFactsHasWorldAndEngine_UsesWorldEngineBlocks()
	{
		var state = new GameState
		{
			Premise = "Premise",
			StorySummary = "Summary",
		};
		state.ApplyKeyFacts("""{"world":{"setting":{"currentLocation":"Forest"}},"engine":{"dangerLevel":"low"}}""");

		var prompt = Narrator.BuildTurnPrompt(state, "look around");

		prompt.Should().Contain("World state (write from this):");
		prompt.Should().Contain("Engine state (obey silently, never mention by name):");
		prompt.Should().NotContain("Game state (JSON):");
	}

	[Fact]
	public void BuildTurnPrompt_WhenKeyFactsLacksWorldEngineStructure_UsesFallbackBlock()
	{
		var state = new GameState
		{
			Premise = "Premise",
			StorySummary = "Summary",
		};
		state.ApplyKeyFacts("""{"setting":{"currentLocation":"Forest"}}""");

		var prompt = Narrator.BuildTurnPrompt(state, "look around");

		prompt.Should().Contain("Game state (JSON):");
		prompt.Should().NotContain("World state (write from this):");
		prompt.Should().NotContain("Engine state (obey silently, never mention by name):");
	}
}
