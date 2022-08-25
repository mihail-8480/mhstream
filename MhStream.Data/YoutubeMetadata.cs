using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtMetadata
{
    [JsonPropertyName("thumbnails")] public YtThumbnail[] Thumbnails { get; set; }

    [JsonPropertyName("formats")] public YtFormat[] Formats { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("webpage_url")] public string WebpageUrl { get; set; }

    [JsonPropertyName("channel")] public string Channel { get; set; }

    [JsonPropertyName("format_id")] public string FormatId { get; set; }

    public string Id { get; set; }
}