using Application;
using Domain.Game;
using IoC;
using Microsoft.Extensions.DependencyInjection;

namespace UI.Console;

class Program
{
	static async Task Main()
	{
		var services = new ServiceCollection();
		services.AddTypingAdventureServices(Config.Configuration);
		var serviceProvider = services.BuildServiceProvider();

		var engine = serviceProvider.GetRequiredService<GameEngine>();
		var state = new GameState();

		// Subscribe to AI call pending event to show user feedback
		engine.OnAiCallPending += (attemptNumber, waitTime) =>
		{
			System.Console.WriteLine($"⏳ AI is thinking... waiting {waitTime.TotalSeconds} seconds");
			return Task.CompletedTask;
		};

		System.Console.WriteLine("Typing Adventure\n");

		System.Console.WriteLine("Enter a theme to begin your story.\n");
		var theme = System.Console.ReadLine();

		var premise = await engine.GeneratePremiseAsync(state, theme);

		System.Console.WriteLine(premise);

		while (!state.GameOver)
		{
			System.Console.Write("\n> ");
			var playerInput = System.Console.ReadLine();

			if (string.IsNullOrWhiteSpace(playerInput))
			{
				continue;
			}

			var response = await engine.ApplyTurnAsync(state, playerInput);

			System.Console.WriteLine("\n" + response);
		}
	}
}

