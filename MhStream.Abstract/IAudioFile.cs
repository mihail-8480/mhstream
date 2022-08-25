namespace MhStream.Abstract;

public interface IAudioFile
{
    public Task<(Stream,IEnumerable<IDisposable>)> GetResource(CancellationToken token);
    public Task<(Stream,IEnumerable<IDisposable>)> GetThumbnail(CancellationToken token);
    public string Title { get; }
    public string Artist { get; }
    public string Id { get; }
}