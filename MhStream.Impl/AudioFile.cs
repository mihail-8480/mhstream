using MhStream.Abstract;

namespace MhStream.Impl;

public class AudioFile : IAudioFile
{
    private readonly Func<Task<(Stream, IEnumerable<IDisposable>)>> _resource;
    private readonly Func<Task<(Stream, IEnumerable<IDisposable>)>> _thumbnail;

    public AudioFile(string title, string artist, string id,
        Func<Task<(Stream, IEnumerable<IDisposable>)>> resource, Func<Task<(Stream, IEnumerable<IDisposable>)>> thumbnail)
    {
        _resource = resource;
        _thumbnail = thumbnail;
        Title = title;
        Artist = artist;
        Id = id;
    }

    public Task<(Stream, IEnumerable<IDisposable>)> GetResource(CancellationToken token)
    {
        return _resource();
    }

    public Task<(Stream, IEnumerable<IDisposable>)> GetThumbnail(CancellationToken token)
    {
        return _thumbnail();
    }

    public string Title { get; }
    public string Artist { get; }
    public string Id { get; }
}