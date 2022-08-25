using System.Diagnostics;
using System.Runtime.CompilerServices;
using MhStream.Abstract;
using MhStream.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MhStream.Impl;

public class YtPlaylistProvider : IPlaylistProvider
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IHttpClientFactory _factory;
    private readonly IMetadataParser<YtMetadata> _metadataParser;
    private readonly IConfiguration _configuration;

    public YtPlaylistProvider(IResourceProvider<ProcessStartInfo> resourceProvider, IHttpClientFactory factory,
        IMetadataParser<YtMetadata> metadataParser, IConfiguration configuration)
    {
        _resourceProvider = resourceProvider;
        _factory = factory;
        _metadataParser = metadataParser;
        _configuration = configuration;
    }

    public async IAsyncEnumerable<IAudioFile> GetFiles(string playlistId,
        [EnumeratorCancellation] CancellationToken token)
    {
        using var processResource = await _resourceProvider.GetResource(new ProcessStartInfo
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


        var jsonReader = new JsonTextReader(new StreamReader(await processResource.GetStream(token)))
        {
            SupportMultipleContent = true
        };

        var jsonSerializer = new JsonSerializer();
        while (await jsonReader.ReadAsync(token))
        {
            var metadata = jsonSerializer.Deserialize<YtMetadata>(jsonReader);
            if (metadata == null) continue;
            yield return new YtAudioFile(metadata.Id, _configuration, metadata, _resourceProvider, _factory,
                _metadataParser);
        }
    }
}