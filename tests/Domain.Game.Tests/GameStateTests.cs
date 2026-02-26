using Domain.Game;
using Xunit;

namespace Domain.Game.Tests;

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
