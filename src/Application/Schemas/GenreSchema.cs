using Domain.Game.Enums;

namespace Application.Schemas;

public static class GenreSchema
{
	public static string For(Genre genre) =>
		genre switch
		{
			Genre.Fantasy => Fantasy.Schema,
			Genre.Horror => Horror.Schema,
			Genre.Mystery => Mystery.Schema,
			Genre.SciFi => SciFi.Schema,
			Genre.Western => Western.Schema,
			_ => Agnostic.Schema
		};
}