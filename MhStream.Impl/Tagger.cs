using MhStream.Abstract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TagLib;

namespace MhStream.Impl;

public class Tagger : ITagger
{
    public async Task<IResource> Tag(IAudioFile metadata, Stream stream, CancellationToken token)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms, token);
        using var thumbnailRes = await metadata.GetThumbnail(token);
        var thumbnailStream = await thumbnailRes.GetStream(token);

        using var image = await Image.LoadAsync(thumbnailStream, token);

        var offset = (image.Width - image.Height) / 2;
        image.Mutate(i => i.Crop(new Rectangle(offset, 0, image.Height, image.Height)));
        var saveStream = new MemoryStream();
        await image.SaveAsync(saveStream, new JpegEncoder(), token);

        ms.Seek(0, SeekOrigin.Begin);
        var tagFile = TagLib.File.Create(new StreamFileAbstraction(metadata.Title + ".mp3", ms,ms));

        var tag = tagFile.GetTag(TagTypes.Id3v2, true);
        tag.Pictures = new IPicture[]
        {
            new Picture(saveStream.ToArray())
        };
        tag.Performers = new[]
        {
            metadata.Artist
        };
        tag.Title = metadata.Title;
        
        tagFile.Save();

        ms.Seek(0, SeekOrigin.Begin);

        return new StreamResource(ms, "audio/mpeg", true);
    }
}