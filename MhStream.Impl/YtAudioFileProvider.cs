using System.Diagnostics;
using System.Text.Json;
using MhStream.Abstract;
using MhStream.Data;

namespace MhStream.Impl;

public class YtAudioFileProvider : IAudioProvider<string>
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IHttpClientFactory _factory;

    public YtAudioFileProvider(IResourceProvider<ProcessStartInfo> resourceProvider, IHttpClientFactory factory)
    {
        _resourceProvider = resourceProvider;
        _factory = factory;
    }
    public async Task<IAudioFile> GetAudioFile(string url, CancellationToken token)
    {
        using var processResource = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = "/usr/bin/youtube-dl",
            ArgumentList =
            {
                "-j",
                url,
            }
        }, "application/json", token);

        var stream = await processResource.GetStream(token);

        var metadata = await JsonSerializer.DeserializeAsync<YtMetadata>(stream, cancellationToken: token);
        return metadata == null ? null : new YtAudioFile(metadata, _resourceProvider, _factory);
    }
}