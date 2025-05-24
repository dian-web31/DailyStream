using Microsoft.Extensions.Configuration;

public class ApiConfig
{
    private readonly IConfiguration _config;

    public ApiConfig(IConfiguration config)
    {
        _config = config;
    }

    public string ClientId => _config["Dailymotion:ClientId"];
    public string ClientSecret => _config["Dailymotion:ClientSecret"];
    public string ApiBaseUrl => _config["Dailymotion:ApiBaseUrl"];
}
