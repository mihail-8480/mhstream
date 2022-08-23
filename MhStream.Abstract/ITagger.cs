namespace MhStream.Abstract;

public interface ITagger
{
    public Task<IResource> Tag(IAudioFile metadata, Stream stream, CancellationToken token);
}