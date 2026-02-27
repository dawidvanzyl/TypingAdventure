using Application.Interfaces;
using FluentAssertions;
using Xunit;

namespace Infrastructure.AI.Tests;

public class AiClientTests
{
    [Fact]
    public void AiClient_ImplementsIAiClient()
    {
        var type = typeof(IAiClient);
        var implType = typeof(AiClient);

        implType.Should().Implement(type);
    }
}
