using MhStream.Abstract;

namespace MhStream.Impl;

public class ResourceProxy : IResource
{
    private readonly IResource _resource;
    private readonly IDisposable _disposable;

    public ResourceProxy(IResource resource, IDisposable disposable)
    {
        _resource = resource;
        _disposable = disposable;
    }

    public void Dispose()
    {
        _resource.Dispose();
        _disposable.Dispose();
    }

    public Task<Stream> GetStream()
    {
        return _resource.GetStream();
    }
}