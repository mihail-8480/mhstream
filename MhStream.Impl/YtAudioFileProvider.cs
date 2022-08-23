using System.Diagnostics;
using System.Text.Json;
using MhStream.Abstract;
using MhStream.Data;

namespace MhStream.Impl;

public class YtAudioFileProvider : IAudioProvider<string>
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;

    public YtAudioFileProvider(IResourceProvider<ProcessStartInfo> resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }
    public async Task<IAudioFile> GetAudioFile(string url)
    {
        using var processResource = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = "/usr/bin/youtube-dl",
            ArgumentList =
            {
                "-j",
                url,
            }
        });

        var stream = await processResource.GetStream();

        var metadata = await JsonSerializer.DeserializeAsync<YtMetadata>(stream);
        return metadata == null ? null : new YtAudioFile(metadata, _resourceProvider);
    }
}