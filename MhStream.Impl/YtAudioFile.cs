using System.Diagnostics;
using MhStream.Abstract;
using MhStream.Data;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MhStream.Impl;

public class YtAudioFile : IAudioFile<YtMetadata>
{
    private readonly IConfiguration _configuration;
    private readonly YtMetadata _metadata;
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IHttpClientFactory _factory;
    private readonly IMetadata _parsedMetadata;

    public YtAudioFile(string id, IConfiguration configuration, YtMetadata metadata,
        IResourceProvider<ProcessStartInfo> resourceProvider, IHttpClientFactory factory,
        IMetadataParser<YtMetadata> metadataParser)
    {
        Id = id;
        _configuration = configuration;
        _metadata = metadata;
        _resourceProvider = resourceProvider;
        _factory = factory;
        _parsedMetadata = metadataParser.Parse(metadata);
    }

    public Task<YtMetadata> GetMetadata(CancellationToken token)
    {
        return Task.FromResult(_metadata);
    }

    private string FindOptimalAudioFormatId()
    {
        var formats = _metadata.Formats.Where(x => x.VCodec == "none").ToArray();

        if (formats.Length == 0)
        {
            formats = _metadata.Formats;
        }

        return formats.MaxBy(x => x.Abr)?.FormatId ?? _metadata.FormatId;
    }

    public Task<IResource> GetResource(CancellationToken token)
    {
        return _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = _configuration["Binaries:youtube-dl"],
            ArgumentList =
            {
                "--quiet",
                "-f",
                FindOptimalAudioFormatId(),
                _metadata.WebpageUrl,
                "-o",
                "-"
            }
        }, "audio/webm", token);
    }

    public async Task<IResource> GetThumbnail(CancellationToken token)
    {
        using var httpClient = _factory.CreateClient();
        var stream = await httpClient.GetStreamAsync(_metadata.Thumbnails.MaxBy(x => x.Height * x.Width)?.Url, token);

        using var image = await Image.LoadAsync(stream, token);

        var offset = (image.Width - image.Height) / 2;
        image.Mutate(i => i.Crop(new Rectangle(offset, 0, image.Height, image.Height)));
        var saveStream = new MemoryStream();
        await image.SaveAsync(saveStream, new JpegEncoder(), token);
        return new StreamResource(saveStream, "image/jpeg", true);
    }

    public string Title => _parsedMetadata.Title;
    public string Artist => _parsedMetadata.Artist;
    public string Id { get; }
}