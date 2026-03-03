using Application.GenreDetection;
using Application.Tests.Fakes;
using Domain.Game.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class GenreDetectorTests
{
	[Fact]
	public async Task DetectAsync_WithFantasyTheme_ReturnsFantasy()
	{
		var fake = new FakeAiClient { NextResponse = "Fantasy" };
		var detector = new GenreDetector(fake);

		var genre = await detector.DetectAsync("a wizard in an enchanted kingdom");

		genre.Should().Be(Genre.Fantasy);
	}

	[Fact]
	public async Task DetectAsync_WithUnrecognisedResponse_ReturnsAgnostic()
	{
		var fake = new FakeAiClient { NextResponse = "Steampunk" };
		var detector = new GenreDetector(fake);

		var genre = await detector.DetectAsync("a city powered by steam engines");

		genre.Should().Be(Genre.Agnostic);
	}

	[Fact]
	public async Task DetectAsync_WithEmptyTheme_ReturnsAgnosticWithoutCallingAi()
	{
		var fake = new FakeAiClient { NextResponse = "Fantasy" };
		var detector = new GenreDetector(fake);

		var genre = await detector.DetectAsync(string.Empty);

		genre.Should().Be(Genre.Agnostic);
		fake.Calls.Should().BeEmpty();
	}

	[Fact]
	public async Task DetectAsync_WithMixedCaseResponse_ReturnsCorrectGenre()
	{
		var fake = new FakeAiClient { NextResponse = "horror" };
		var detector = new GenreDetector(fake);

		var genre = await detector.DetectAsync("a haunted house on a stormy night");

		genre.Should().Be(Genre.Horror);
	}
}