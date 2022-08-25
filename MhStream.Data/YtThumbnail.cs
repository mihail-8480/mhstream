using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtThumbnail
{
    [JsonPropertyName("width")] public int? Width { get; set; }

    [JsonPropertyName("height")] public int? Height { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}