namespace MhStream.Abstract;

public interface IDataUrlProvider
{
    public Task<(string, IEnumerable<IDisposable>)> GetDataUrl(Stream resource, CancellationToken token);
}