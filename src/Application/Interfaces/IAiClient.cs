namespace Application.Interfaces;

public interface IAiClient
{
    Task<string> GetCompletionAsync(string systemPrompt, string userPrompt);
}
