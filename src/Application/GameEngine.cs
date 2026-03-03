using System.Text.Json;
using Application.Exceptions;
using Application.GenreDetection;
using Application.Interfaces;
using Application.Prompts;
using Application.Schemas;
using Domain.Game;

namespace Application;

public class GameEngine
{
    private readonly IAiClient _aiClient;
    private readonly GenreDetector _genreDetector;

    public event AiCallPendingHandler OnAiCallPending;

    public GameEngine(IAiClient aiClient, GenreDetector genreDetector)
    {
        _aiClient = aiClient;
        _genreDetector = genreDetector;
        _aiClient.OnAiCallPending += (attemptNumber, waitTime) =>
        {
            OnAiCallPending?.Invoke(attemptNumber, waitTime);
            return Task.CompletedTask;
        };
    }

    public async Task<string> GeneratePremiseAsync(GameState state, string theme)
    {
        state.DetectedGenre = await _genreDetector.DetectAsync(theme);

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

        await AddKeyFactsJsonAsync(state);

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

    private async Task<string> GetKeyFactsJsonAsync(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var latestTurn = state.StoryLog.LastOrDefault() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(latestTurn))
        {
            return "{}";
        }

        var isFreshExtraction = string.IsNullOrWhiteSpace(state.KeyFactsJson)
            || state.KeyFactsJson == "{}";

        var userPrompt = isFreshExtraction
            ? KeyFacts.BuildKeyFactsPrompt(latestTurn)
            : KeyFacts.BuildUpdatePrompt(state.KeyFactsJson, latestTurn);

        string aiResponse;
        try
        {
            aiResponse = await _aiClient.GetCompletionAsync(
                KeyFacts.BuildSystemPrompt(GenreSchema.For(state.DetectedGenre)),
                userPrompt);
        }
        catch (Exception ex)
        {
            throw new KeyFactExtractionException("Failed to extract key facts from AI.", ex);
        }

        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return state.KeyFactsJson;
        }

        try
        {
            JsonDocument.Parse(aiResponse);
        }
        catch (JsonException)
        {
            return state.KeyFactsJson;
        }

        return aiResponse;
    }

	private async Task AddKeyFactsJsonAsync(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var keyFactsJson = await GetKeyFactsJsonAsync(state);
        state.KeyFactsJson = keyFactsJson;
    }
}

