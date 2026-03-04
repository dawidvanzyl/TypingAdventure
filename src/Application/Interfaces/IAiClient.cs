namespace Application.Interfaces;

public delegate Task AiCallPendingHandler(int attemptNumber, TimeSpan waitTime);

public interface IAiClient
{
	Task<string> GetCompletionAsync(string systemPrompt, string userPrompt);

	event AiCallPendingHandler OnAiCallPending;
}

