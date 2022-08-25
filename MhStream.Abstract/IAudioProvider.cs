namespace MhStream.Abstract;

public interface IAudioProvider<in T>
{
    public Task<(IAudioFile,IEnumerable<IDisposable>)> GetAudioFile(T url, CancellationToken token);
}