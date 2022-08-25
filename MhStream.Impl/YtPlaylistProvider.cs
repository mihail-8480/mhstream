using System.Diagnostics;
using System.Runtime.CompilerServices;
using MhStream.Abstract;
using MhStream.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MhStream.Impl;

public class YtPlaylistProvider : IPlaylistProvider
{
    private readonly IResourceProvider<ProcessStartInfo, Process> _resourceProvider;
    private readonly IHttpClientFactory _factory;
    private readonly IMetadataParser<YtMetadata> _metadataParser;
    private readonly IConfiguration _configuration;

    public YtPlaylistProvider(IResourceProvider<ProcessStartInfo, Process> resourceProvider, IHttpClientFactory factory,
        IMetadataParser<YtMetadata> metadataParser, IConfiguration configuration)
    {
        _resourceProvider = resourceProvider;
        _factory = factory;
        _metadataParser = metadataParser;
        _configuration = configuration;
    }

    public async IAsyncEnumerable<(IAudioFile, IEnumerable<IDisposable>)> GetFiles(string playlistId,
        [EnumeratorCancellation] CancellationToken token)
    {
        var (process, disposables) = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = _configuration["Binaries:youtube-dl"],
            ArgumentList =
            {
                "--quiet",
                "-j",
                "-i",
                $"{playlistId}"
            }
        }, "application/json", token);

        yield return (null, disposables);

        using var jsonReader = new JsonTextReader(new StreamReader(process.StandardOutput.BaseStream))
        {
            SupportMultipleContent = true
        };

        var jsonSerializer = new JsonSerializer();
        while (await jsonReader.ReadAsync(token))
        {
            var metadata = jsonSerializer.Deserialize<YtMetadata>(jsonReader);
            if (metadata == null) continue;
            yield return (new YtAudioFile(metadata.Id, _configuration, metadata, _resourceProvider, _factory,
                _metadataParser), ArraySegment<IDisposable>.Empty);
        }

    }
}