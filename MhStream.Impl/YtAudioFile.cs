using System.Diagnostics;
using System.Text;
using MhStream.Abstract;
using MhStream.Data;

namespace MhStream.Impl;

public class YtAudioFile : IAudioFile<YtMetadata>
{
    private readonly YtMetadata _metadata;
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IHttpClientFactory _factory;

    public YtAudioFile(YtMetadata metadata, IResourceProvider<ProcessStartInfo> resourceProvider, IHttpClientFactory factory)
    {
        _metadata = metadata;
        _resourceProvider = resourceProvider;
        _factory = factory;
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
            FileName = "/usr/bin/youtube-dl",
            ArgumentList =
            {
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
        return new StreamResource(stream, "image/jpeg",true);
    }


    private string TrimmedTitle()
    {
        return _metadata.Title
            .Replace("official music video", "", StringComparison.CurrentCultureIgnoreCase)
            .Replace("official video", "", StringComparison.CurrentCultureIgnoreCase)
            .Replace("[]", "")
            .Replace("()", "");
    }
    
    private string GetTitle()
    {
        var title = TrimmedTitle();
        if (title.Contains(']'))
        {
            title = title.Split(']')[1];
        }
        if (title.Contains('『'))
        {
            var split = title.Split("』");
            return split[0].Split("『")[1].Trim();
        }
        if (!title.Contains('-')) return title;
        var index = title.IndexOf('-');
        return title[(index + 1)..].Trim();

    }

    private string GetArtist()
    {
        var title = TrimmedTitle();
        if (title.Contains(']'))
        {
            title = title.Split(']')[1];
        }
        if (title.Contains('『'))
        {
            var builder = new StringBuilder();
            var split = title.Split("』");
            builder.Append(split[0].Split("『")[0]);
            foreach (var part in split.Skip(1))
            {
                builder.Append(' ');
                builder.Append(part);
            }

            return builder.ToString().Trim();

        }

        if (!title.Contains('-')) return _metadata.Channel;
        var index = title.IndexOf('-');
        return title[..index].Trim();
    }

    public string Title => GetTitle();
    public string Artist => GetArtist();
}