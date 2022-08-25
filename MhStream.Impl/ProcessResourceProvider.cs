using System.Collections.Immutable;
using System.Diagnostics;
using MhStream.Abstract;

namespace MhStream.Impl;

public class ProcessResourceProvider : IResourceProvider<ProcessStartInfo, Process>
{
    public Task<(Process, IEnumerable<IDisposable>)> GetResource(ProcessStartInfo resource, string contentType, CancellationToken token)
    {
        resource.RedirectStandardOutput = true;
        resource.RedirectStandardInput = true;
        var process = Process.Start(resource);


        return process != null
            ? Task.FromResult<(Process, IEnumerable<IDisposable>)>((process, new []{new Disposable(() =>
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    process.Dispose();
                }
            })}))
            : Task.FromResult<(Process, IEnumerable<IDisposable>)>((null, ImmutableArray<IDisposable>.Empty));
    }
}