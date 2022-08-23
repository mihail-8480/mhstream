using MhStream.Abstract;

namespace MhStream.Impl;

public class AudioFileProxy : IAudioFile
{
    private readonly IResource _resource;
    private readonly IAudioFile _audioFile;

    public AudioFileProxy(IResource resource, IAudioFile audioFile)
    {
        _resource = resource;
        _audioFile = audioFile;
    }

    public Task<IResource> GetResource()
    {
        return Task.FromResult(_resource);
    }

    public Task<IResource> GetThumbnail()
    {
        return _audioFile.GetThumbnail();
    }
}