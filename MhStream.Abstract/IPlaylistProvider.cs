namespace MhStream.Abstract;

public interface IPlaylistProvider
{
    public IAsyncEnumerable<(IAudioFile, IEnumerable<IDisposable>)> GetFiles(string playlistId, CancellationToken token);
}