using System.Text.Json.Serialization;

namespace DailyStream.Models.ApiResponses;

public class VideoResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("list")]
    public List<Video> List { get; set; } = new List<Video>();
}
