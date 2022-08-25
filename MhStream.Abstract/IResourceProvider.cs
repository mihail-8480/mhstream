namespace MhStream.Abstract;

public interface IResourceProvider<in T, TResource>
{
    public Task<(TResource,IEnumerable<IDisposable>)> GetResource(T resource, string contentType, CancellationToken token);
}