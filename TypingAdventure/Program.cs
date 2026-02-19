namespace TypingAdventure;

class Program
{
    static async Task Main()
    {
       Console.WriteLine("Typing Adventure\n");        

        var ai = new AiClient(Config.Configuration);
        var state = new GameState();

        Console.WriteLine("Enter a theme to begin your story.\n");
        var theme = Console.ReadLine();

        // 1. Generate premise
        var premise = await ai.GetCompletionAsync(
            "You are a creative story narrator.",
            PromptBuilder.BuildPremisePrompt(theme));

        state.Premise = premise;
        state.StoryLog.Add(premise);

        Console.WriteLine(premise);

        // Initial summary
        state.StorySummary = await Summarise(ai, state);

        // 2. Main loop
        while (!state.GameOver)
        {
            Console.Write("\n> ");
            var playerInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(playerInput))
                continue;

            var turnPrompt = PromptBuilder.BuildTurnPrompt(state, playerInput);

            var response = await ai.GetCompletionAsync(
                PromptBuilder.NarratorSystemPrompt,
                turnPrompt);

            state.StoryLog.Add(response);

            Console.WriteLine("\n" + response);

            // Update summary every turn
            state.StorySummary = await Summarise(ai, state);
        }
    }

    static async Task<string> Summarise(AiClient ai, GameState state)
    {
        var fullStory = string.Join("\n\n", state.StoryLog);

        return await ai.GetCompletionAsync(
            "You summarise stories accurately.",
            PromptBuilder.BuildSummaryPrompt(fullStory));
    }

    static async Task<string> KeyFacts(AiClient ai, GameState state)
    {
        var fullStory = string.Join("\n\n", state.StoryLog);

        return await ai.GetCompletionAsync(
            "You keep track of the key facts in short hand form.",
            PromptBuilder.BuildSummaryPrompt(fullStory));
    }
}
