
namespace MhStream.Abstract;

public interface IAudioFile
{
    public Task<IResource> GetResource();
    public Task<IResource> GetThumbnail();
}

public interface IAudioFile<T> : IAudioFile
{
    public Task<T> GetMetadata();
}