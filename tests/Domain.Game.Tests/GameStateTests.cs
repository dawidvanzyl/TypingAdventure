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
}
