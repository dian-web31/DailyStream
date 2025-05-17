using System.Text.Json;
using DailyStream.Models.ApiResponses;

namespace DailyStream.Services;

public class DailymotionAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ApiConfig _config;
    private string _accessToken;
    private DateTime _tokenExpiration;

    public DailymotionAuthService(HttpClient httpClient, ApiConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
        {
            return _accessToken;
        }

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _config.ClientId),
            new KeyValuePair<string, string>("client_secret", _config.ClientSecret)
        });

        var response = await _httpClient.PostAsync($"{_config.ApiBaseUrl}/oauth/token", requestData);
        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync();
        var tokenResponse = await JsonSerializer.DeserializeAsync<AuthTokenResponse>(responseStream);

        _accessToken = tokenResponse.AccessToken;
        _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);


        return _accessToken;
    }
}
