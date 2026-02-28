using Application.Exceptions;
using Application.Interfaces;
using Application.Prompts;
using Domain.Game;

namespace Application;

public class GameEngine
{
	private readonly IAiClient _aiClient;

	public event AiCallPendingHandler OnAiCallPending;

	public GameEngine(IAiClient aiClient)
	{
		_aiClient = aiClient;
		_aiClient.OnAiCallPending += (attemptNumber, waitTime) =>
		{
			OnAiCallPending?.Invoke(attemptNumber, waitTime);
			return Task.CompletedTask;
		};
	}

    public async Task<string> GeneratePremiseAsync(GameState state, string theme)
    {
        var premise = await _aiClient.GetCompletionAsync(
            "You are a creative story narrator.",
            Narrator.BuildPremisePrompt(theme ?? string.Empty));

		state.Premise = premise;
		state.StoryLog.Add(premise);

		state.StorySummary = await SummariseAsync(state);

		return premise;
	}

    public async Task<string> ApplyTurnAsync(GameState state, string playerInput)
    {
        var turnPrompt = Narrator.BuildTurnPrompt(state, playerInput);

        var response = await _aiClient.GetCompletionAsync(
            Narrator.SystemPrompt,
            turnPrompt);

		state.StoryLog.Add(response);

		state.StorySummary = await SummariseAsync(state);

        await AddKeyFactsAsync(state);

        return response;
    }

	public async Task<string> SummariseAsync(GameState state)
	{
		var fullStory = string.Join("\n\n", state.StoryLog);

        var summary = await _aiClient.GetCompletionAsync(
            "You summarise stories accurately.",
            Narrator.BuildSummaryPrompt(fullStory));

		state.StorySummary = summary;

        return summary;
    }

    private async Task<IReadOnlyCollection<string>> GetKeyFactsAsync(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var fullStory = string.Join("\n\n", state.StoryLog);

        if (string.IsNullOrWhiteSpace(fullStory))
        {
            return Array.Empty<string>();
        }

        string aiResponse;
        try
        {
            aiResponse = await _aiClient.GetCompletionAsync(
                KeyFacts.SystemPrompt,
                KeyFacts.BuildKeyFactsPrompt(fullStory));
        }
        catch (Exception ex)
        {
            throw new KeyFactExtractionException("Failed to extract key facts from AI.", ex);
        }

        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return Array.Empty<string>();
        }

        var extractedFacts = aiResponse
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        var newFacts = extractedFacts
            .Except(state.KeyFacts, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return newFacts.AsReadOnly();
    }

    private async Task AddKeyFactsAsync(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var newFacts = await GetKeyFactsAsync(state);
        var distinctFacts = newFacts.Except(state.KeyFacts, StringComparer.OrdinalIgnoreCase);
        state.KeyFacts.AddRange(distinctFacts);
    }
}

