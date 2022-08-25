using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class ProcessResource : IResource, IPipe
{
    private readonly Process _process;
    private readonly string _contentType;

    public ProcessResource(Process process, string contentType)
    {
        _process = process;
        _contentType = contentType;
    }

    public Task<Stream> GetStream(CancellationToken token)
    {
        return Task.FromResult(_process.StandardOutput.BaseStream);
    }

    public string GetContentType()
    {
        return _contentType;
    }

    public IResource Pipe()
    {
        return new StreamResource(_process.StandardInput.BaseStream, _contentType, false);
    }

    public void Dispose()
    {
        _process.Dispose();
    }
}