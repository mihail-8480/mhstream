namespace MhStream.Abstract;

public interface IAudioConvert
{
    public Task<IResource> Convert(IAudioFile file);
}