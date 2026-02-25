using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TrendyolClone.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                SetHeaders(headers);
                
                _logger.LogInformation($"GET Request: {url}");
                var response = await _httpClient.GetAsync(url);
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response Status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {content}");
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }
                
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GET request failed for URL: {url}");
                throw;
            }
        }

        public async Task<string> PostAsync(string url, object data, Dictionary<string, string> headers = null)
        {
            try
            {
                SetHeaders(headers);
                
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation($"POST Request: {url}");
                _logger.LogDebug($"POST Data: {json}");
                
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Response Status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {responseContent}");
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }
                
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"POST request failed for URL: {url}");
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            var json = await GetAsync(url, headers);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T> PostAsync<T>(string url, object data, Dictionary<string, string> headers = null)
        {
            var json = await PostAsync(url, data, headers);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void SetHeaders(Dictionary<string, string> headers)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void SetApiKey(string keyName, string keyValue)
        {
            _httpClient.DefaultRequestHeaders.Add(keyName, keyValue);
        }
    }
}