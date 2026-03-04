using Application.Interfaces;

namespace Application.Tests.Fakes;

public class FakeAiClient : IAiClient
{
	public List<AiClientCall> Calls { get; } = new();

	public Queue<string> NextResponses { get; } = new();

	public string NextResponse { get; set; } = "Response";

	public event AiCallPendingHandler OnAiCallPending;

	public Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
	{
		Calls.Add(new AiClientCall(systemPrompt, userPrompt));

		return NextResponses.Count > 0
			? Task.FromResult(NextResponses.Dequeue()) 
			: Task.FromResult(NextResponse);
	}

	internal void FireAiCallPending()
	{
		OnAiCallPending?.Invoke(1, TimeSpan.FromSeconds(2));
	}
}

