using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class ProcessResourceProvider : IResourceProvider<ProcessStartInfo>
{
    public Task<IResource> GetResource(ProcessStartInfo resource, string contentType, CancellationToken token)
    {
        resource.RedirectStandardOutput = true;
        resource.RedirectStandardInput = true;
        var process = Process.Start(resource);
        token.Register(() =>
        {
            try
            {
                if (process is not {HasExited: false}) return;
                process.Kill();
            }
            catch
            {
                // ignored
            }
        });
        
        return process != null
            ? Task.FromResult<IResource>(new ProcessResource(process, contentType))
            : Task.FromResult<IResource>(null);
    }
}