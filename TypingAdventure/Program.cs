namespace TypingAdventure;

class Program
{
    static async Task Main()
    {
       Console.WriteLine("Typing Adventure\n");        

        var ai = new AiClient(Config.Configuration);
        var engine = new GameEngine(ai);
        var state = new GameState();

        Console.WriteLine("Enter a theme to begin your story.\n");
        var theme = Console.ReadLine();

        // 1. Generate premise
        var premise = await engine.GeneratePremiseAsync(state, theme);

        Console.WriteLine(premise);

        // 2. Main loop
        while (!state.GameOver)
        {
            Console.Write("\n> ");
            var playerInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(playerInput))
                continue;

            var response = await engine.ApplyTurnAsync(state, playerInput);

            Console.WriteLine("\n" + response);

        }
    }
}
