using Application;
using Application.Interfaces;
using Domain.Game;
using Xunit;

namespace Application.Tests;

public class GameEngineTests
{
    [Fact]
    public async Task GeneratePremiseAsync_UpdatesStateAndUsesTheme()
    {
        var fake = new FakeAiClient();
        fake.NextResponse = "Generated premise";
        var engine = new GameEngine(fake);
        var state = new GameState();

        var premise = await engine.GeneratePremiseAsync(state, "mystery");

        Assert.Equal("Generated premise", premise);
        Assert.Equal("Generated premise", state.Premise);
        Assert.Contains("Generated premise", state.StoryLog);
        Assert.NotEmpty(state.StorySummary);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Contains("Generate a unique mystery premise", fake.Calls[0].UserPrompt);
    }

    [Fact]
    public async Task ApplyTurnAsync_AppendsResponseAndRefreshesSummary()
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

        Assert.Equal("Turn response", response);
        Assert.Contains("Turn response", state.StoryLog);
        Assert.Equal("Updated summary", state.StorySummary);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Contains("Last player action:", fake.Calls[0].UserPrompt);
        Assert.Contains("look around", fake.Calls[0].UserPrompt);
    }

    [Fact]
    public async Task SummariseAsync_UsesFullStoryInPrompt()
    {
        var fake = new FakeAiClient();
        fake.NextResponse = "Summary";
        var engine = new GameEngine(fake);
        var state = new GameState();
        state.StoryLog.Add("Line 1");
        state.StoryLog.Add("Line 2");

        var summary = await engine.SummariseAsync(state);

        Assert.Equal("Summary", summary);
        Assert.Equal("Summary", state.StorySummary);
        Assert.Single(fake.Calls);
        var call = fake.Calls[0];
        Assert.Contains("Line 1", call.UserPrompt);
        Assert.Contains("Line 2", call.UserPrompt);
    }
}

public class FakeAiClient : IAiClient
{
    public record Call(string SystemPrompt, string UserPrompt);

    public List<Call> Calls { get; } = new();

    public Queue<string> NextResponses { get; } = new();

    public string NextResponse { get; set; } = "Response";

    public Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
    {
        Calls.Add(new Call(systemPrompt, userPrompt));

        if (NextResponses.Count > 0)
        {
            return Task.FromResult(NextResponses.Dequeue());
        }

        return Task.FromResult(NextResponse);
    }
}
