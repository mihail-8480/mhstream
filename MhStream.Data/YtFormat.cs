using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtFormat
{
    [JsonPropertyName("format_id")]
    public string FormatId { get; set; }
    [JsonPropertyName("vcodec")]
    public string VCodec { get; set; }

    [JsonPropertyName("abr")]
    public float? Abr { get; set; }


}