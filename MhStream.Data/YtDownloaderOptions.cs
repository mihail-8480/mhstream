using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtDownloaderOptions
{
    [JsonPropertyName("http_chunk_size")]
    public int? HttpChunkSize { get; set; }
}