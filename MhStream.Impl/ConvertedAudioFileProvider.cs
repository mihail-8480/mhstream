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

    public async Task<(IAudioFile,IEnumerable<IDisposable>)> GetAudioFile(T url, CancellationToken token)
    {
        var (audioFile, disposables0) = await _audioProvider.GetAudioFile(url, token);
        var (converted,disposables1) = await _convert.Convert(audioFile, token);
        return (new AudioFile(audioFile.Title, audioFile.Artist, audioFile.Id, () => 
            Task.FromResult<(Stream, IEnumerable<IDisposable>)>((converted, ArraySegment<IDisposable>.Empty)), 
            () => audioFile.GetThumbnail(CancellationToken.None)), disposables0.Concat(disposables1));
    }
}