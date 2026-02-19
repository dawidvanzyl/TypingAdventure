using System.Net.Http.Headers;

namespace Cerebras
{
    public class CerebrasClient
    {
        private HttpClient _httpClient;

        public CerebrasClient(ApiKeyCredential credential, CerebrasClientOptions options)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = options.Endpoint,
                Timeout = options.Timeout
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ChatClient GetChatClient(string model)
        {
            return new ChatClient(_httpClient, model);
        }
    }
}
