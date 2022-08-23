using MhStream.Abstract;

namespace MhStream.Impl;

public class StreamResource : IResource
{
    private readonly Stream _stream;
    private readonly string _contentType;
    private readonly bool _close;

    public StreamResource(Stream stream, string contentType, bool close)
    {
        _stream = stream;
        _contentType = contentType;
        _close = close;
    }
    public Task<Stream> GetStream(CancellationToken token)
    {
        return Task.FromResult(_stream);
    }

    public string GetContentType()
    {
        return _contentType;
    }

    public void Dispose()
    {
        if (_close)
        {
            _stream.Dispose();
        }
    }
}