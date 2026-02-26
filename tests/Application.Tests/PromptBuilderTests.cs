using Application;
using Domain.Game;
using Xunit;

namespace Application.Tests;

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
