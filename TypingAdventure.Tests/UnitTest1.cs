using TypingAdventure;

namespace TypingAdventure.Tests;

public class PromptBuilderTests
{
    [Fact]
    public void BuildPremisePrompt_IncludesThemeAndQuestion()
    {
        var theme = "space opera";

        var prompt = PromptBuilder.BuildPremisePrompt(theme);

        Assert.Contains(theme, prompt);
        Assert.Contains("what they do first", prompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildTurnPrompt_IncludesCoreSections()
    {
        var state = new GameState
        {
            Premise = "Premise",
            StorySummary = "Summary"
        };
        state.KeyFacts.Add("Fact1");
        state.Inventory.Add("Sword");
        var playerInput = "attack";

        var prompt = PromptBuilder.BuildTurnPrompt(state, playerInput);

        Assert.Contains("Game premise:", prompt);
        Assert.Contains(state.Premise, prompt);
        Assert.Contains("Story summary:", prompt);
        Assert.Contains(state.StorySummary, prompt);
        Assert.Contains("Known facts:", prompt);
        Assert.Contains("Fact1", prompt);
        Assert.Contains("Inventory:", prompt);
        Assert.Contains("Sword", prompt);
        Assert.Contains("Last player action:", prompt);
        Assert.Contains(playerInput, prompt);
    }

    [Fact]
    public void NarratorSystemPrompt_ContainsKeyRules()
    {
        var systemPrompt = PromptBuilder.NarratorSystemPrompt;

        Assert.Contains("Write in second person", systemPrompt);
        Assert.Contains("Do not decide actions for the player", systemPrompt);
        Assert.Contains("What do you do?", systemPrompt);
    }
}

public class GameStateTests
{
    [Fact]
    public void DefaultState_HasExpectedDefaults()
    {
        var state = new GameState();

        Assert.Equal(string.Empty, state.Premise);
        Assert.Equal(string.Empty, state.StorySummary);
        Assert.False(state.GameOver);
        Assert.Empty(state.StoryLog);
        Assert.Empty(state.KeyFacts);
        Assert.Empty(state.Inventory);
    }
}

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

