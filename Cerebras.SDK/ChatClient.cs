using System.Text.Json;

namespace Cerebras.SDK;

public class ChatClient
{
	private readonly HttpClient _httpClient;
	private readonly string _model;

	public ChatClient(HttpClient httpClient, string model)
	{
		_httpClient = httpClient;
		_model = model;
	}

	public async Task<CompletionResponse> CompleteChatAsync(IEnumerable<Message> messages, ChatCompletionOptions options)
	{
		var requestBody = new
		{
			model = _model,
			messages = messages.Select(m => new
			{
				role = m.Role,
				content = m.Content
			}),
			max_tokens = options.MaxTokens,
			temperature = options.Temperature
		};

		var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}/chat/completions")
		{
			Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
		};

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		var completionResponse = JsonSerializer.Deserialize<CompletionResponse>(responseBody);

		return completionResponse;
	}
}
