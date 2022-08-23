using MhStream.Abstract;

namespace MhStream.Impl;

public class StreamResource : IResource
{
    private readonly Stream _stream;
    private readonly bool _close;

    public StreamResource(Stream stream, bool close)
    {
        _stream = stream;
        _close = close;
    }
    public Task<Stream> GetStream()
    {
        return Task.FromResult(_stream);
    }

    public void Dispose()
    {
        if (_close)
        {
            _stream.Dispose();
        }
    }
}