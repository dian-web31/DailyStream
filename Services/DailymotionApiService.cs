using System.Net.Http.Headers;
using DailyStream.Models;
using DailyStream.Models.ApiResponses;
using System.Text.Json;

namespace DailyStream.Services;

public class DailymotionApiService
{
    private readonly HttpClient _httpClient;
    private readonly DailymotionAuthService _authService;
    private readonly ApiConfig _config;

    public DailymotionApiService(
        HttpClient httpClient,
        DailymotionAuthService authService,
        ApiConfig config)
    {
        _httpClient = httpClient;
        _authService = authService;
        _config = config;
    }

    public async Task<VideoResponse> SearchVideosAsync(string query, int limit = 16, int page = 1)
    {
        var token = await _authService.GetAccessTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(
            $"{_config.ApiBaseUrl}/videos?limit={limit}&page={page}&search={Uri.EscapeDataString(query)}&fields=id,title,channel,description,url,duration,views_total");

        response.EnsureSuccessStatusCode();
        
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var videoResponse = await JsonSerializer.DeserializeAsync<VideoResponse>(responseStream);
        
        // Obtener thumbnails para cada video
        foreach (var video in videoResponse.List)
        {
            await EnrichVideoWithThumbnailAsync(video);
        }
        
        return videoResponse;
    }
    
    public async Task<List<Video>> GetPopularVideosAsync(int count = 12)
    {
        var token = await _authService.GetAccessTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(
            $"{_config.ApiBaseUrl}/videos?sort=trending&limit={count}&fields=id,title,channel,description,tags,url,duration,views_total");

        response.EnsureSuccessStatusCode();
        
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var videoResponse = await JsonSerializer.DeserializeAsync<VideoResponse>(responseStream);
        
        // Obtener thumbnails para cada video
        foreach (var video in videoResponse.List)
        {
            await EnrichVideoWithThumbnailAsync(video);
        }
        
        return videoResponse.List;
    }
    
    public async Task<Video> GetVideoByIdAsync(string id)
    {
        var token = await _authService.GetAccessTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(
            $"{_config.ApiBaseUrl}/video/{id}?fields=id,title,channel,description,tags,url,duration,views_total");

        response.EnsureSuccessStatusCode();
        
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var video = await JsonSerializer.DeserializeAsync<Video>(responseStream);
        
        await EnrichVideoWithThumbnailAsync(video);
        
        return video;
    }
    
    private async Task EnrichVideoWithThumbnailAsync(Video video)
    {
        if (video == null || string.IsNullOrEmpty(video.Id))
            return;
            
        var token = await _authService.GetAccessTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(
            $"{_config.ApiBaseUrl}/video/{video.Id}?fields=thumbnail_small_url,thumbnail_medium_url,thumbnail_large_url");

        response.EnsureSuccessStatusCode();
        
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var thumbnailData = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream);
        
        if (thumbnailData.TryGetProperty("thumbnail_small_url", out var smallUrl))
            video.ThumbnailSmallUrl = smallUrl.GetString();
            
        if (thumbnailData.TryGetProperty("thumbnail_medium_url", out var mediumUrl))
            video.ThumbnailMediumUrl = mediumUrl.GetString();
            
        if (thumbnailData.TryGetProperty("thumbnail_large_url", out var largeUrl))
            video.ThumbnailLargeUrl = largeUrl.GetString();
    }
}
