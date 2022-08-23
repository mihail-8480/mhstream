using System.Text.Json.Serialization;

namespace MhStream.Data;

public class YtMetadata
{
    
    [JsonPropertyName("thumbnails")]
    public YtThumbnail[] Thumbnails { get; set; }
    
    [JsonPropertyName("categories")]
    public string[] Categories { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("formats")]
    public YtFormat[] Formats { get; set; }
    
    [JsonPropertyName("requested_formats")]
    public YtFormat[] RequestedFormats { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("upload_date")]
    public string UploadDate { get; set; }
    
    [JsonPropertyName("uploader")]
    public string Uploader { get; set; }
    
    [JsonPropertyName("uploader_id")]
    public string UploaderId { get; set; }
    
    [JsonPropertyName("uploader_url")]
    public string UploaderUrl { get; set; }
    
    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; }
    
    [JsonPropertyName("channel_url")]
    public string ChannelUrl { get; set; }
    
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
    
    [JsonPropertyName("view_count")]
    public int? ViewCount { get; set; }
    
    [JsonPropertyName("age_limit")]
    public int? AgeLimit { get; set; }
    
    [JsonPropertyName("webpage_url")]
    public string WebpageUrl { get; set; }
    
    [JsonPropertyName("like_count")]
    public int? LikeCount { get; set; }
    
    [JsonPropertyName("channel")]
    public string Channel { get; set; }
    
    [JsonPropertyName("extractor")]
    public string Extractor { get; set; }
    
    [JsonPropertyName("webpage_url_basename")]
    public string WebpageUrlBaseName { get; set; }
    
    [JsonPropertyName("extractor_key")]
    public string ExtractorKey { get; set; }
    
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
    
    [JsonPropertyName("format")]
    public string Format { get; set; }
    
    [JsonPropertyName("format_id")]
    public string FormatId { get; set; }
    
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    
    [JsonPropertyName("fps")]
    public int? Fps { get; set; }
    
    [JsonPropertyName("vcodec")]
    public string VCodec { get; set; }
    
    [JsonPropertyName("vbr")]
    public float? Vbr { get; set; }
    
    [JsonPropertyName("acodec")]
    public string ACodec { get; set; }
    
    [JsonPropertyName("abr")]
    public float? Abr { get; set; }
    
    [JsonPropertyName("ext")]
    public string Ext { get; set; }
    
    [JsonPropertyName("fulltitle")]
    public string FullTitle { get; set; }
    
    [JsonPropertyName("_filename")]
    public string Filename { get; set; }
}