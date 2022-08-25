using System.Diagnostics;
using MhStream.Abstract;
using Microsoft.Extensions.Configuration;

namespace MhStream.Impl;

public class FfmpegMp3Convert : IAudioConvert
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;
    private readonly IConfiguration _configuration;

    public FfmpegMp3Convert(IResourceProvider<ProcessStartInfo> resourceProvider, IConfiguration configuration)
    {
        _resourceProvider = resourceProvider;
        _configuration = configuration;
    }

    public async Task<IResource> Convert(IAudioFile file, CancellationToken token)
    {
        using var audioResource = await file.GetResource(token);
        var audioStream = await audioResource.GetStream(token);
        var ffmpeg = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = _configuration["Binaries:ffmpeg"],
            ArgumentList =
            {
                "-i",
                "-",
                "-vn",
                "-ar",
                "44100",
                "-ac",
                "2",
                "-b:a",
                "192k",
                "-f",
                "mp3",
                "pipe:1",
                "-hide_banner",
                "-loglevel",
                "error"
            }
        }, "audio/mpeg", token);

        if (ffmpeg is not IPipe pipe) return null;
        var ffmpegInput = pipe.Pipe();
        var ffmpegOutput = await ffmpeg.GetStream(token);
        var ffmpegInputStream = await ffmpegInput.GetStream(token);
        _ = Task.Run(() => audioStream.CopyToAsync(ffmpegInputStream, token).ContinueWith(_ =>
        {
            ffmpegInputStream.Close();
            audioResource.Dispose();
            return Task.CompletedTask;
        }, token), token);

        return new ResourceProxy(new StreamResource(ffmpegOutput, "audio/mpeg", false), ffmpeg);
    }
}