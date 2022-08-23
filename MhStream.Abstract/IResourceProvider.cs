namespace MhStream.Abstract;

public interface IResourceProvider<in T>
{
    public Task<IResource> GetResource(T resource, string contentType, CancellationToken token);
}