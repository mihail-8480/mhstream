using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class FfmpegMp3Convert : IAudioConvert
{
    private readonly IResourceProvider<ProcessStartInfo> _resourceProvider;

    public FfmpegMp3Convert(IResourceProvider<ProcessStartInfo> resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }
    public async Task<IResource> Convert(IAudioFile file)
    {
        var audioResource = await file.GetResource();
        var audioStream = await audioResource.GetStream(); 
        var ffmpeg = await _resourceProvider.GetResource(new ProcessStartInfo
        {
            FileName = "/usr/bin/ffmpeg",
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
                "pipe:1"
            }
        });

        if (ffmpeg is not IPipe pipe) return null;
        var ffmpegInput = pipe.Pipe();
        var ffmpegOutput = await ffmpeg.GetStream();
        var ffmpegInputStream = await ffmpegInput.GetStream();
        _ = Task.Run(() => audioStream.CopyToAsync(ffmpegInputStream).ContinueWith(_ =>
        {
            ffmpegInputStream.Close();
            audioResource.Dispose();
            return Task.CompletedTask;
        }));

        return new ResourceProxy(new StreamResource(ffmpegOutput, false), ffmpeg);

    }
}