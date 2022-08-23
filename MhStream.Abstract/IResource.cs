namespace MhStream.Abstract;

public interface IResource : IDisposable
{
    public Task<Stream> GetStream(CancellationToken token);
}