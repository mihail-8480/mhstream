namespace MhStream.Abstract;

public interface IAudioConvert
{
    public Task<(Stream, IEnumerable<IDisposable>)> Convert(IAudioFile file, CancellationToken token);
}