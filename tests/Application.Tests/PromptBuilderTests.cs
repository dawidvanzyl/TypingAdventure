using Application;
using Domain.Game;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class PromptBuilderTests
{
    [Fact]
    public void BuildPremisePrompt_WithTheme_IncludesThemeAndQuestion()
    {
        var theme = "space opera";

        var prompt = PromptBuilder.BuildPremisePrompt(theme);

        prompt.Should().Contain(theme);
        prompt.Should().Contain("what they do first");
    }

    [Fact]
    public void BuildTurnPrompt_WithGameState_IncludesCoreSections()
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

        prompt.Should().Contain("Game premise:");
        prompt.Should().Contain(state.Premise);
        prompt.Should().Contain("Story summary:");
        prompt.Should().Contain(state.StorySummary);
        prompt.Should().Contain("Known facts:");
        prompt.Should().Contain("Fact1");
        prompt.Should().Contain("Inventory:");
        prompt.Should().Contain("Sword");
        prompt.Should().Contain("Last player action:");
        prompt.Should().Contain(playerInput);
    }

    [Fact]
    public void NarratorSystemPrompt_ContainsKeyRules()
    {
        var systemPrompt = PromptBuilder.NarratorSystemPrompt;

        systemPrompt.Should().Contain("Write in second person");
        systemPrompt.Should().Contain("Do not decide actions for the player");
        systemPrompt.Should().Contain("What do you do?");
    }
}
