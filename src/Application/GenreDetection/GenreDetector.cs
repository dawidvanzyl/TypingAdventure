using Application.Interfaces;
using Domain.Game.Enums;

namespace Application.GenreDetection;

public class GenreDetector(IAiClient aiClient)
{
	private readonly IAiClient _aiClient = aiClient;

	public async Task<Genre> DetectAsync(string theme)
	{
		if (string.IsNullOrWhiteSpace(theme))
		{
			return Genre.Agnostic;
		}

		try
		{
			var response = await _aiClient.GetCompletionAsync(
				"""
				You are a genre classification engine for a text-adventure game.
				Given a story theme, respond with exactly one word: the genre.
				Valid genres: Fantasy, Horror, Mystery, SciFi, Western, Agnostic.
				If the theme does not clearly match any genre, respond with: Agnostic.
				Respond with only the genre word, nothing else.
				""",
				$"Theme: {theme}");

			var trimmed = response?.Trim() ?? string.Empty;

			return Enum.TryParse<Genre>(trimmed, ignoreCase: true, out var detected)
				? detected
				: Genre.Agnostic;
		}
		catch
		{
			return Genre.Agnostic;
		}
	}
}