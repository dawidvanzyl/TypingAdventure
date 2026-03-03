using Application;
using Application.GenreDetection;
using Application.Tests.Fakes;
using Domain.Game;
using Domain.Game.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class GameEngineTests
{
	[Fact]
	public async Task GeneratePremiseAsync_WithValidTheme_UpdatesStateAndReturnsPremise()
	{
		var fake = new FakeAiClient();
		fake.NextResponses.Enqueue("Fantasy");
		fake.NextResponses.Enqueue("Generated premise");
		fake.NextResponses.Enqueue("Generated summary");
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState();

		var premise = await engine.GeneratePremiseAsync(state, "mystery");

		premise.Should().Be("Generated premise");
		state.Premise.Should().Be("Generated premise");
		state.StoryLog.Should().Contain("Generated premise");
		state.StorySummary.Should().NotBeEmpty();
		state.DetectedGenre.Should().Be(Genre.Fantasy);
		fake.Calls.Should().HaveCount(3);
		fake.Calls[1].UserPrompt.Should().Contain("Generate a unique mystery premise");
	}

	[Fact]
	public async Task ApplyTurnAsync_WithPlayerAction_AppendsResponseAndRefreshesSummary()
	{
		var fake = new FakeAiClient();
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState
		{
			Premise = "Premise",
			StorySummary = "Summary"
		};
		state.StoryLog.Add("Premise");
		fake.NextResponses.Enqueue("Turn response");
		fake.NextResponses.Enqueue("Updated summary");
		fake.NextResponses.Enqueue("""
			{
			  "setting": { },
			  "characters": { },
			  "objects": { },
			  "narrative": { },
			  "flags": { }
			}
			""");

		var response = await engine.ApplyTurnAsync(state, "look around");

		response.Should().Be("Turn response");
		state.StoryLog.Should().Contain("Turn response");
		state.StorySummary.Should().Be("Updated summary");
		state.KeyFactsJson.Should().NotBeEmpty();
		fake.Calls.Should().HaveCount(3);
		fake.Calls[0].UserPrompt.Should().Contain("Last player action:");
		fake.Calls[0].UserPrompt.Should().Contain("look around");
	}

	[Fact]
	public async Task SummariseAsync_WithFullStory_UsesFullStoryInPrompt()
	{
		var fake = new FakeAiClient { NextResponse = "Summary" };
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState();
		state.StoryLog.Add("Line 1");
		state.StoryLog.Add("Line 2");

		var summary = await engine.SummariseAsync(state);

		summary.Should().Be("Summary");
		state.StorySummary.Should().Be("Summary");
		fake.Calls.Should().ContainSingle();
		var call = fake.Calls[0];
		call.UserPrompt.Should().Contain("Line 1");
		call.UserPrompt.Should().Contain("Line 2");
	}

	[Fact]
	public void GameEngine_FiresOnAiCallPendingEvent()
	{
		var fake = new FakeAiClient();
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var eventFired = false;

		engine.OnAiCallPending += (attemptNumber, waitTime) =>
		{
			eventFired = true;
			return Task.CompletedTask;
		};

		fake.FireAiCallPending();

		eventFired.Should().BeTrue();
	}
}