using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class ProcessResource : IResource, IPipe
{
    private readonly Process _process;

    public ProcessResource(Process process)
    {
        _process = process;
    }
    
    public Task<Stream> GetStream(CancellationToken token)
    {
        return Task.FromResult(_process.StandardOutput.BaseStream);
    }

    public IResource Pipe()
    {
        return new StreamResource(_process.StandardInput.BaseStream, false);
    }

    public void Dispose()
    {
        _process.Dispose();
    }
}