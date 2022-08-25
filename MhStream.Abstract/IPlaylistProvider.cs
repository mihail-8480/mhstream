namespace MhStream.Abstract;

public interface IPlaylistProvider
{
    public IAsyncEnumerable<IAudioFile> GetFiles(string playlistId, CancellationToken token);
}