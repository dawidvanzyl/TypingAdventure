using Application.Interfaces;
using Xunit;

namespace Infrastructure.AI.Tests;

public class AiClientTests
{
    [Fact]
    public void AiClient_ImplementsIAiClient()
    {
        var type = typeof(IAiClient);
        var implType = typeof(AiClient);

        Assert.True(type.IsAssignableFrom(implType));
    }
}
