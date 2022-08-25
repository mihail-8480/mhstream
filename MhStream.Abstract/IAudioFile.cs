namespace MhStream.Abstract;

public interface IAudioFile
{
    public Task<IResource> GetResource(CancellationToken token);
    public Task<IResource> GetThumbnail(CancellationToken token);
    public string Title { get; }
    public string Artist { get; }
    public string Id { get; }
}

public interface IAudioFile<T> : IAudioFile
{
    public Task<T> GetMetadata(CancellationToken token);
}