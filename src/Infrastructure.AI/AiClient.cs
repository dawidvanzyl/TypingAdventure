using Microsoft.Extensions.Configuration;
using Cerebras.SDK;
using Application.Interfaces;
using Polly;

namespace Infrastructure.AI;

public class AiClient : IAiClient
{
	private readonly CerebrasClient _client;
	private readonly IConfiguration _configuration;
	private readonly IAsyncPolicy<string> _retryPolicy;

	public event AiCallPendingHandler? OnAiCallPending;

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
		_retryPolicy = BuildRetryPolicy();
	}

	public async Task<string> GetCompletionAsync(
		string systemPrompt,
		string userPrompt)
	{
		return await _retryPolicy.ExecuteAsync(async () =>
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
		});
	}

	private IAsyncPolicy<string> BuildRetryPolicy()
	{
		var waits = new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(20) };
		var attempt = 0;

		return Policy<string>
			.Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
			.Or<TaskCanceledException>()
			.WaitAndRetryAsync(
				retryCount: 3,
				sleepDurationProvider: _ =>
				{
					var waitTime = waits[attempt];
					attempt++;
					return waitTime;
				},
				onRetryAsync: async (outcome, timeSpan, retryCount, context) =>
				{
					OnAiCallPending?.Invoke(retryCount, timeSpan);
					await Task.CompletedTask;
				});
	}
}

