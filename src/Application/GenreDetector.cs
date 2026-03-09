using Application.Interfaces;
using Domain.Game.Enums;
using GenreDetectorPrompts = Application.Prompts.GenreDetector;

namespace Application;

public class GenreDetector(IAiClient aiClient)
{
	private readonly IAiClient _aiClient = aiClient;

	public async Task<Genre> DetectAsync(string theme)
	{
		if (string.IsNullOrWhiteSpace(theme))
		{
			return Genre.Agnostic;
		}

		var response = await _aiClient.GetCompletionAsync(
			GenreDetectorPrompts.SystemPrompt,
			GenreDetectorPrompts.BuildDetectPrompt(theme));

		var trimmed = response?.Trim() ?? string.Empty;

		return Enum.TryParse<Genre>(trimmed, ignoreCase: true, out var detected)
			? detected
			: Genre.Agnostic;
	}
}
