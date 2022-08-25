using MhStream.Abstract;

namespace MhStream.Impl;

public record ParsedMetadata(string Title, string Artist) : IMetadata;