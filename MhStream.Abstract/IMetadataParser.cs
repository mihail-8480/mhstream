namespace MhStream.Abstract;

public interface IMetadataParser<in T>
{
    IMetadata Parse(T parse);
}