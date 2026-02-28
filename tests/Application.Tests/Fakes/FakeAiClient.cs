using Application.Interfaces;
using Application.Tests.Fakes;

namespace Application.Tests.Fakes;

public class FakeAiClient : IAiClient
{
	public List<AiClientCall> Calls { get; } = new();

	public Queue<string> NextResponses { get; } = new();

	public string NextResponse { get; set; } = "Response";

	public event AiCallPendingHandler? OnAiCallPending;

	public Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
	{
		Calls.Add(new AiClientCall(systemPrompt, userPrompt));

		if (NextResponses.Count > 0)
		{
			return Task.FromResult(NextResponses.Dequeue());
		}

		return Task.FromResult(NextResponse);
	}
}

