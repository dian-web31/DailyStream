using System.Text.Json.Serialization;
namespace DailyStream.Models;

public class Video
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("channel")]
    public string? Channel { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
    
    [JsonPropertyName("thumbnail_small_url")]
    public string? ThumbnailSmallUrl { get; set; }
    
    [JsonPropertyName("thumbnail_medium_url")]
    public string? ThumbnailMediumUrl { get; set; }
    
    [JsonPropertyName("thumbnail_large_url")]
    public string? ThumbnailLargeUrl { get; set; }
    
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("views_total")]
    public int ViewsTotal { get; set; }
    
    public int Likes { get; set; } = new Random().Next(1, 1000); // Simulación de likes
}
