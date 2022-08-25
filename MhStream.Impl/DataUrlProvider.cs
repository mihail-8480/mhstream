using System.Text;
using MhStream.Abstract;

namespace MhStream.Impl;

public class DataUrlProvider : IDataUrlProvider
{
    public async Task<string> GetDataUrl(IResource resource, CancellationToken token)
    {
        var stream = await resource.GetStream(token);
        if (stream is not MemoryStream)
        {
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms, token);
            stream = ms;
        }
        
        await using var memoryStream = stream as MemoryStream;
        var array = memoryStream.ToArray();

        var sb = new StringBuilder();
        sb.Append("data:")
            .Append(resource.GetContentType())
            .Append(";base64,")
            .Append(Convert.ToBase64String(array));

        return sb.ToString();
    }
}