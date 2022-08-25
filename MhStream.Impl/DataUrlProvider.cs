using System.Text;
using MhStream.Abstract;

namespace MhStream.Impl;

public class DataUrlProvider : IDataUrlProvider
{
    public async Task<(string, IEnumerable<IDisposable>)> GetDataUrl(Stream resource, CancellationToken token)
    {
        if (resource is not MemoryStream)
        {
            var ms = new MemoryStream();
            await resource.CopyToAsync(ms, token);
            await resource.DisposeAsync();
            resource = ms;
        }

        await using var memoryStream = (MemoryStream) resource;
        var array = memoryStream.ToArray();

        var sb = new StringBuilder();
        sb.Append("data:")
            .Append("image/jpeg")
            .Append(";base64,")
            .Append(Convert.ToBase64String(array));

        return (sb.ToString(), new[]
        {
            resource
        });
    }
}