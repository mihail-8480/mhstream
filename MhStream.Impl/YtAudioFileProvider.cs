using System.Diagnostics;
using System.Text.Json;
using MhStream.Abstract;
using MhStream.Data;
using Microsoft.Extensions.Configuration;

namespace MhStream.Impl;

public class YtAudioFileProvider : IAudioProvider<string>
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _factory;
    private readonly IMetadataParser<YtMetadata> _metadataParser;

    public YtAudioFileProvider(IResourceProvider<ProcessStartInfo> resourceProvider, IConfiguration configuration,
        IHttpClientFactory factory, IMetadataParser<YtMetadata> metadataParser)
    {
        _resourceProvider = resourceProvider;
        _configuration = configuration;
        _factory = factory;
        _metadataParser = metadataParser;
    }

    public async Task<IAudioFile> GetAudioFile(string url, CancellationToken token)
    {
        using var processResource = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = _configuration["Binaries:youtube-dl"],
            ArgumentList =
            {
                "--quiet",
                "-j",
                $"https://www.youtube.com/watch?v={url}",
            }
        }, "application/json", token);

        var stream = await processResource.GetStream(token);

        var metadata = await JsonSerializer.DeserializeAsync<YtMetadata>(stream, cancellationToken: token);
        return metadata == null
            ? null
            : new YtAudioFile(url, _configuration, metadata, _resourceProvider, _factory, _metadataParser);
    }
}