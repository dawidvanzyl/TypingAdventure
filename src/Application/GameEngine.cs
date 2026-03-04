using System.Text.Json;
using Application.Exceptions;
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
		ArgumentNullException.ThrowIfNull(state);
		state.DetectedGenre = await _genreDetector.DetectAsync(theme);

		var premise = await _aiClient.GetCompletionAsync(
			"You are a creative story narrator.",
			Narrator.BuildPremisePrompt(theme ?? string.Empty));

		state.Premise = premise;
		state.StoryLog.Add(premise);

		state.StorySummary = await SummariseAsync(state);

		await UpdateKeyFactsAsync(state, KeyFacts.BuildKeyFactsPrompt(premise));

		return premise;
	}

	public async Task<string> ApplyTurnAsync(GameState state, string playerInput)
	{
		ArgumentNullException.ThrowIfNull(state);

		var turnPrompt = Narrator.BuildTurnPrompt(state, playerInput);

		var response = await _aiClient.GetCompletionAsync(
			Narrator.SystemPrompt,
			turnPrompt);

		state.StoryLog.Add(response);

		state.StorySummary = await SummariseAsync(state);

		await UpdateKeyFactsAsync(state, KeyFacts.BuildUpdatePrompt(state.KeyFactsJson, response));

		return response;
	}

	public async Task<string> SummariseAsync(GameState state)
	{
		ArgumentNullException.ThrowIfNull(state);

		var fullStory = string.Join("\n\n", state.StoryLog);

		var summary = await _aiClient.GetCompletionAsync(
			"You summarise stories accurately.",
			Narrator.BuildSummaryPrompt(fullStory));

		state.StorySummary = summary;

		return summary;
	}

	private async Task UpdateKeyFactsAsync(GameState state, string userPrompt)
	{
		ArgumentNullException.ThrowIfNull(state);

		if (string.IsNullOrWhiteSpace(userPrompt))
		{
			return;
		}

		try
		{
			var aiResponse = await _aiClient.GetCompletionAsync(
				KeyFacts.BuildSystemPrompt(GenreSchema.For(state.DetectedGenre)),
				userPrompt);

			if (string.IsNullOrWhiteSpace(aiResponse))
			{
				return;
			}

			JsonDocument.Parse(aiResponse);
			state.KeyFactsJson = aiResponse;
		}
		catch (JsonException)
		{
			return;
		}
		catch (Exception ex)
		{
			throw new KeyFactExtractionException("Failed to extract key facts from AI.", ex);
		}
	}
}
