using Domain.Game;
using Application.Interfaces;

namespace Application;

public class GameEngine
{
    private readonly IAiClient _aiClient;

    public GameEngine(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }

    public async Task<string> GeneratePremiseAsync(GameState state, string theme)
    {
        var premise = await _aiClient.GetCompletionAsync(
            "You are a creative story narrator.",
            PromptBuilder.BuildPremisePrompt(theme ?? string.Empty));

        state.Premise = premise;
        state.StoryLog.Add(premise);

        state.StorySummary = await SummariseAsync(state);

        return premise;
    }

    public async Task<string> ApplyTurnAsync(GameState state, string playerInput)
    {
        var turnPrompt = PromptBuilder.BuildTurnPrompt(state, playerInput);

        var response = await _aiClient.GetCompletionAsync(
            PromptBuilder.NarratorSystemPrompt,
            turnPrompt);

        state.StoryLog.Add(response);

        state.StorySummary = await SummariseAsync(state);

        return response;
    }

    public async Task<string> SummariseAsync(GameState state)
    {
        var fullStory = string.Join("\n\n", state.StoryLog);

        var summary = await _aiClient.GetCompletionAsync(
            "You summarise stories accurately.",
            PromptBuilder.BuildSummaryPrompt(fullStory));

        state.StorySummary = summary;

        return summary;
    }
}
