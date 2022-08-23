using System.Diagnostics;
using MhStream.Abstract;
using MhStream.Data;

namespace MhStream.Impl;

public class YtAudioFile : IAudioFile<YtMetadata>
{
    private readonly YtMetadata _metadata;
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;

    public YtAudioFile(YtMetadata metadata, IResourceProvider<ProcessStartInfo> resourceProvider)
    {
        _metadata = metadata;
        _resourceProvider = resourceProvider;
    }

    public Task<YtMetadata> GetMetadata()
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
    
    public Task<IResource> GetResource()
    {
        return _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = "/usr/bin/youtube-dl",
            ArgumentList =
            {
                "-f",
                FindOptimalAudioFormatId(),
                _metadata.WebpageUrl,
                "-o",
                "-"
            }
        });    
    }

    public Task<IResource> GetThumbnail()
    {
        throw new NotImplementedException();
    }
}