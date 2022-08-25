using System.Diagnostics;
using MhStream.Abstract;
using Microsoft.Extensions.Configuration;

namespace MhStream.Impl;

public class FfmpegMp3Convert : IAudioConvert
{
    private readonly IResourceProvider<ProcessStartInfo, Process> _resourceProvider;
    private readonly IConfiguration _configuration;

    public FfmpegMp3Convert(IResourceProvider<ProcessStartInfo, Process> resourceProvider, IConfiguration configuration)
    {
        _resourceProvider = resourceProvider;
        _configuration = configuration;
    }

    public async Task<(Stream,IEnumerable<IDisposable>)> Convert(IAudioFile file, CancellationToken token)
    {
        var (audioStream, disposable0) = await file.GetResource(token);
        var (ffmpeg, disposable1) = await _resourceProvider.GetResource(new ProcessStartInfo
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

        var ffmpegInputStream = ffmpeg.StandardInput.BaseStream;
        var ffmpegOutputStream = ffmpeg.StandardOutput.BaseStream;

        _ = Task.Run(async() =>
        {        
            await audioStream.CopyToAsync(ffmpegInputStream, token);
            ffmpegInputStream.Close();
        }, token);
        
        return (ffmpegOutputStream, disposable0.Concat(disposable1));
    }
}