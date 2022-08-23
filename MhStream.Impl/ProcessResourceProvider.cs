using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class ProcessResourceProvider : IResourceProvider<ProcessStartInfo>
{
    public Task<IResource> GetResource(ProcessStartInfo resource, CancellationToken token)
    {
        resource.RedirectStandardOutput = true;
        resource.RedirectStandardInput = true;
        var process = Process.Start(resource);
        token.Register(() =>
        {
            if (!process?.HasExited == true)
            {
                process.Kill();
            }
        });
        
        return process != null
            ? Task.FromResult<IResource>(new ProcessResource(process))
            : Task.FromResult<IResource>(null);
    }
}