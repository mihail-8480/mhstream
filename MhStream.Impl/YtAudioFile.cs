using System.Diagnostics;
using MhStream.Abstract;
using MhStream.Data;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MhStream.Impl;

public class YtAudioFile : IAudioFile
{
    private readonly IConfiguration _configuration;
    private readonly YtMetadata _metadata;
    private readonly IResourceProvider<ProcessStartInfo, Process> _resourceProvider;
    private readonly IHttpClientFactory _factory;
    private readonly IMetadata _parsedMetadata;

    public YtAudioFile(string id, IConfiguration configuration, YtMetadata metadata,
        IResourceProvider<ProcessStartInfo, Process> resourceProvider, IHttpClientFactory factory,
        IMetadataParser<YtMetadata> metadataParser)
    {
        Id = id;
        _configuration = configuration;
        _metadata = metadata;
        _resourceProvider = resourceProvider;
        _factory = factory;
        _parsedMetadata = metadataParser.Parse(metadata);
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

    public async Task<(Stream, IEnumerable<IDisposable>)> GetResource(CancellationToken token)
    {
        var (process, enumerable) = await _resourceProvider.GetResource(new ProcessStartInfo
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
        return (process.StandardOutput.BaseStream, enumerable);
    }

    public async Task<(Stream, IEnumerable<IDisposable>)> GetThumbnail(CancellationToken token)
    {
        using var httpClient = _factory.CreateClient();
        await using var stream = await httpClient.GetStreamAsync(_metadata.Thumbnails.MaxBy(x => x.Height * x.Width)?.Url, token);

        using var image = await Image.LoadAsync(stream, token);

        var offset = (image.Width - image.Height) / 2;
        image.Mutate(i => i.Crop(new Rectangle(offset, 0, image.Height, image.Height)));
        
        var saveStream = new MemoryStream();
        await image.SaveAsync(saveStream, new JpegEncoder(), token);
        
        return (saveStream, new []
        {
            saveStream
        });
    }

    public string Title => _parsedMetadata.Title;
    public string Artist => _parsedMetadata.Artist;
    public string Id { get; }
}