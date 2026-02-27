using Application.Interfaces;
using FluentAssertions;
using Infrastructure.AI.Tests.Helpers;
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

	[Fact]
	public void AiClient_HasOnAiCallPendingEvent()
	{
		// Act
		var eventInfo = typeof(AiClient).GetEvent("OnAiCallPending");

		// Assert
		eventInfo.Should().NotBeNull();
		eventInfo.EventHandlerType.Should().Be(typeof(AiCallPendingHandler));
	}

	[Fact]
	public async Task GetCompletionAsync_CanBeInvoked()
	{
		// Arrange
		var configuration = new MockConfiguration();
		var client = new AiClient(configuration);

		// Act & Assert - should not throw
		try
		{
			await client.GetCompletionAsync("system", "user");
		}
		catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
		{
			// These exceptions are expected since we're not mocking the actual API
		}
	}

	[Fact]
	public void OnAiCallPending_CanBeSubscribedTo()
	{
		// Arrange
		var configuration = new MockConfiguration();
		var client = new AiClient(configuration);
		var eventFired = false;
		var capturedAttempt = 0;
		var capturedWaitTime = TimeSpan.Zero;

		client.OnAiCallPending += (attemptNumber, waitTime) =>
		{
			eventFired = true;
			capturedAttempt = attemptNumber;
			capturedWaitTime = waitTime;
			return Task.CompletedTask;
		};

		// Act - verify subscription succeeded (no exception thrown)

		// Assert
		eventFired.Should().BeFalse(); // Event shouldn't fire until actual retry occurs
	}
}

