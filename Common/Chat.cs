using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//"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"
namespace RazorEnhanced
{
    public class ChatExample
    {
        private readonly HuggingFaceApiClient _chatService;
    
        public ChatExample(string apiToken)
        {
            _chatService = new HuggingFaceApiClient(apiToken);
        }

        public string GetResponse(string message)
        {
            try
            {
                return _chatService.GenerateTextAsync(message).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
    
	public class HuggingFaceApiClient
    {
        private readonly HttpClient _client;
        private readonly string _apiToken;
        private DateTime _lastRequestTime = DateTime.MinValue;
        
        public HuggingFaceApiClient(string apiToken)
        {
            if (string.IsNullOrEmpty(apiToken))
                throw new ArgumentException("API token cannot be empty", nameof(apiToken));
                
            _client = new HttpClient();
            _apiToken = apiToken;
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            // Basic rate limiting
            var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
            if (timeSinceLastRequest.TotalMilliseconds < 200)
                await Task.Delay(200 - (int)timeSinceLastRequest.TotalMilliseconds);

            try
            {
                var request = new
                {
                    inputs = prompt,
                    parameters = new
                    {
                        max_length = 512,
                        temperature = 0.7
                    }
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json");

                _lastRequestTime = DateTime.UtcNow;
                var response = await _client.PostAsync(
                    "https://api-inference.huggingface.co/models/microsoft/phi-2",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API request failed: {response.StatusCode}, {errorContent}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                return result[0].generated_text;
            }
            catch (Exception ex)
            {
                throw new Exception($"Text generation failed: {ex.Message}", ex);
            }
        }
    }



}