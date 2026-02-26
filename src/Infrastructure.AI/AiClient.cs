using Microsoft.Extensions.Configuration;
using Cerebras.SDK;
using Application.Interfaces;

namespace Infrastructure.AI;

public class AiClient : IAiClient
{
    private readonly CerebrasClient _client;
    private readonly IConfiguration _configuration;

    public AiClient(IConfiguration configuration)
    {
        _client = new CerebrasClient(
            new ApiKeyCredential(configuration["AiClient:ApiKey"]),
            new CerebrasClientOptions
            {
                Endpoint = new Uri(configuration["AiClient:EndPoint"]),
                Timeout = TimeSpan.FromSeconds(30)
            });
        _configuration = configuration;
    }

    public async Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt)
    {
        try
        {
            var messages = new Message[]
            {
                new Message { Role = "system", Content = systemPrompt },
                new Message { Role = "user", Content = userPrompt },
            };

            var maxTokens = _configuration.GetValue<int>("AiClient:MaxTokens");
            var temperature = _configuration.GetValue<float>("AiClient:Temperature");

            var options = new ChatCompletionOptions
            {
                MaxTokens = maxTokens,
                Temperature = temperature
            };

            var completion = await _client
                .GetChatClient(_configuration["AiClient:Model"])
                .CompleteChatAsync(messages, options);

            return completion.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}
