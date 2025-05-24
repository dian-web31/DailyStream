using System.Text.Json.Serialization;

namespace DailyStream.Models.ApiResponses;

public class AuthTokenResponse
{

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
