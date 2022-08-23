using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtFormat
{
    [JsonPropertyName("downloader_options")]
    public YtDownloaderOptions DownloaderOptions { get; set; }
    
    [JsonPropertyName("tags")]
    public string[] Tags { get; set; }

    [JsonPropertyName("container")]
    public string Container { get; set; }
    
    [JsonPropertyName("format")]
    public string Format { get; set; }
    
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; }
    
    [JsonPropertyName("http_headers")]
    public Dictionary<string, string> HttpHeaders { get; set; }

    [JsonPropertyName("asr")]
    public int? Asr { get; set; }
    
    [JsonPropertyName("filesize")]
    public int? Filesize { get; set; }
    
    [JsonPropertyName("format_id")]
    public string FormatId { get; set; }
    
    [JsonPropertyName("format_note")]
    public string FormatNote { get; set; }
    
    [JsonPropertyName("fps")]
    public int? Fps { get; set; }
    
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }
    
    [JsonPropertyName("tbr")]
    public float? Tbr { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("ext")]
    public string Ext { get; set; }

    [JsonPropertyName("vcodec")]
    public string VCodec { get; set; }
    
    [JsonPropertyName("acodec")]
    public string ACodec { get; set; }
    
    [JsonPropertyName("abr")]
    public float? Abr { get; set; }


}