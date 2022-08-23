
namespace MhStream.Abstract;

public interface IAudioProvider<in T>
{
    public Task<IAudioFile> GetAudioFile(T url, CancellationToken token);
}