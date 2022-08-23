using MhStream.Abstract;

namespace MhStream.Impl;

public class ConvertedAudioFileProvider<T> : IAudioProvider<T>
{
    private readonly IAudioProvider<T> _audioProvider;
    private readonly IAudioConvert _convert;

    public ConvertedAudioFileProvider(IAudioProvider<T> audioProvider, IAudioConvert convert)
    {
        _audioProvider = audioProvider;
        _convert = convert;
    }
    
    public async Task<IAudioFile> GetAudioFile(T url, CancellationToken token)
    {
        var audioFile = await _audioProvider.GetAudioFile(url, token);
        var converted = await _convert.Convert(audioFile, token);
        return new AudioFileProxy(converted, audioFile);
    }
}