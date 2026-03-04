using Application;
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
		fake.NextResponses.Enqueue("""{"setting": {"currentLocation": "Forest"}}""");
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState();

		var premise = await engine.GeneratePremiseAsync(state, "mystery");

		premise.Should().Be("Generated premise");
		state.Premise.Should().Be("Generated premise");
		state.StoryLog.Should().Contain("Generated premise");
		state.StorySummary.Should().NotBeEmpty();
		state.DetectedGenre.Should().Be(Genre.Fantasy);
		fake.Calls.Should().HaveCount(4);
		
		var genreDetectionCall = fake.Calls[0];
		genreDetectionCall.UserPrompt.Should().Contain("mystery");

		var premiseCall = fake.Calls[1];
		premiseCall.UserPrompt.Should().Contain("Generate a unique mystery premise");

		var summaryCall = fake.Calls[2];
		summaryCall.UserPrompt.Should().Contain("Summarise");
		summaryCall.UserPrompt.Should().Contain("Generated premise");
		
		var keyFactsCall = fake.Calls[3];
		keyFactsCall.UserPrompt.Should().Contain("Generated premise");
		state.KeyFactsJson.Should().Contain("Forest");
	}

	[Fact]
	public async Task ApplyTurnAsync_WithExistingKeyFacts_UsesUpdatePromptWithCurrentJson()
	{
		var existingJson = """{"setting": {"currentLocation": "Hall"}}""";
		var fake = new FakeAiClient();
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState
		{
			Premise = "Premise",
			StorySummary = "Summary",
			KeyFactsJson = existingJson
		};
		state.StoryLog.Add("Premise");
		fake.NextResponses.Enqueue("Turn response");
		fake.NextResponses.Enqueue("Updated summary");
		fake.NextResponses.Enqueue("""{"setting": {"currentLocation": "Library"}}""");

		await engine.ApplyTurnAsync(state, "go north");

		var keyFactsCall = fake.Calls[2];
		keyFactsCall.UserPrompt.Should().Contain(existingJson);
		keyFactsCall.UserPrompt.Should().Contain("Turn response");
		state.KeyFactsJson.Should().Contain("Library");
	}

	[Fact]
	public async Task ApplyTurnAsync_WhenAiReturnsInvalidJson_RetainsPreviousKeyFacts()
	{
		var existingJson = """{"setting": {"currentLocation": "Hall"}}""";
		var fake = new FakeAiClient();
		var engine = new GameEngine(fake, new GenreDetector(fake));
		var state = new GameState
		{
			Premise = "Premise",
			StorySummary = "Summary",
			KeyFactsJson = existingJson
		};
		state.StoryLog.Add("Premise");
		fake.NextResponses.Enqueue("Turn response");
		fake.NextResponses.Enqueue("Updated summary");
		fake.NextResponses.Enqueue("not valid json at all");

		await engine.ApplyTurnAsync(state, "look around");

		state.KeyFactsJson.Should().Be(existingJson);
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
