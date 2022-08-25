namespace MhStream.Abstract;

public interface IDataUrlProvider
{
    public Task<string> GetDataUrl(IResource resource, CancellationToken token);
}