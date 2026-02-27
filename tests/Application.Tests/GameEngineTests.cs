using Application;
using Application.Interfaces;
using Application.Tests.Fakes;
using Domain.Game;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class GameEngineTests
{
    [Fact]
    public async Task GeneratePremiseAsync_WithValidTheme_UpdatesStateAndReturnsPremise()
    {
        var fake = new FakeAiClient();
        fake.NextResponse = "Generated premise";
        var engine = new GameEngine(fake);
        var state = new GameState();

        var premise = await engine.GeneratePremiseAsync(state, "mystery");

        premise.Should().Be("Generated premise");
        state.Premise.Should().Be("Generated premise");
        state.StoryLog.Should().Contain("Generated premise");
        state.StorySummary.Should().NotBeEmpty();
        fake.Calls.Should().HaveCount(2);
        fake.Calls[0].UserPrompt.Should().Contain("Generate a unique mystery premise");
    }

    [Fact]
    public async Task ApplyTurnAsync_WithPlayerAction_AppendsResponseAndRefreshesSummary()
    {
        var fake = new FakeAiClient();
        var engine = new GameEngine(fake);
        var state = new GameState
        {
            Premise = "Premise",
            StorySummary = "Summary"
        };
        state.StoryLog.Add("Premise");
        fake.NextResponses.Enqueue("Turn response");
        fake.NextResponses.Enqueue("Updated summary");

        var response = await engine.ApplyTurnAsync(state, "look around");

        response.Should().Be("Turn response");
        state.StoryLog.Should().Contain("Turn response");
        state.StorySummary.Should().Be("Updated summary");
        fake.Calls.Should().HaveCount(2);
        fake.Calls[0].UserPrompt.Should().Contain("Last player action:");
        fake.Calls[0].UserPrompt.Should().Contain("look around");
    }

    [Fact]
    public async Task SummariseAsync_WithFullStory_UsesFullStoryInPrompt()
    {
        var fake = new FakeAiClient();
        fake.NextResponse = "Summary";
        var engine = new GameEngine(fake);
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
}
