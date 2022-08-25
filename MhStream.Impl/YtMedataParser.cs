using MhStream.Abstract;
using MhStream.Data;

namespace MhStream.Impl;

public class YtMedataParser : IMetadataParser<YtMetadata>
{
    private string TrimmedTitle(YtMetadata ytMetadata)
    {
        return Trim(ytMetadata.Title
                .Replace("official music audio", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official audio", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official hd audio", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official hd music audio", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official music video", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official video", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official hd video", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official hd music video", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("hd video", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("official", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("[hd]", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("(hd)", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("[]", "")
                .Replace("()", "")
                .Replace(" ＰＶ", "")
                .Replace(" PV", "")
                .Replace(" MV", "")
        );
    }

    private string Trim4(string s)
    {
        return s.Replace("Music", "").Replace("Official", "").Trim();
    }
    
    private string Trim(string s)
    {
        while (true)
        {
            var first = s.Trim().Trim('\'', '\"').Trim();
            if (!first.StartsWith('[') || !first.Contains(']'))
            {
                if (first.EndsWith(']') && first.Contains('['))
                {
                    first = first.Split('[')[0];
                }
                
                return first.Trim('-', ':', '|').Replace('[', '(').Replace(']', ')');
            }

            s = first[(first.IndexOf(']') + 1)..];
        }
    }

    private IEnumerable<string> Trim3(string s)
    {
        foreach (var part in s.Split('('))
        {
            yield return Trim(part.Trim().Trim(')', '-').Trim());
        }
    }
    private string Trim2(string s)
    {
        var aggregated = Trim3(s).Aggregate((x, y) => x + " (" + y + ")");
        var fix = aggregated.Count(x => x == '(') != aggregated.Count(x => x == ')') ? aggregated.TrimEnd(')') : aggregated;

        return fix.Replace("' ", " ").Replace("\" ", " ");
    }
    
    private bool SplitsBy(string title, string s, out ParsedMetadata metadata)
    {
        if (title.Contains(s))
        {
            var idx = title.IndexOf(s, StringComparison.Ordinal);
            metadata = new ParsedMetadata(Trim2(Trim(title[(idx+1)..])), Trim4(Trim(title[..idx])));
            return true;
        }

        metadata = null;
        return false;
    }

    private bool MatchesJp1(string title, out ParsedMetadata metadata)
    {
        if (title.StartsWith("【") && title.Contains("】"))
        {
            var split = title.Split('】');
            var artist = split[0].TrimStart('【');
            var name = split[1];

            metadata = new ParsedMetadata(Trim2(Trim(name)), Trim4(Trim(artist)));
            return true;
        }

        metadata = null;
        return false;
    }

    private bool MatchesJp2(string title, out ParsedMetadata metadata)
    {
        if (title.Contains("「") && title.Contains("」"))
        {
            var split = title.Split('「');
            var name = split[1].Split('」')[0];
            var artist = title.Split('「')[0];
            metadata = new ParsedMetadata(Trim2(Trim(name)), Trim4(Trim(artist)));
            return true;

        }
        
        metadata = null;
        return false;

    }
    
    public IMetadata Parse(YtMetadata parse)
    {
        var title = TrimmedTitle(parse);

        if (SplitsBy(title, "-", out var m1))
        {
            return m1;
        }

        if (SplitsBy(title, ":", out var m2))
        {
            return m2;
        }

        if (MatchesJp1(title, out var m3))
        {
            return m3;
        }

        if (MatchesJp2(title, out var m4))
        {
            return m4;
        }
        
        return new ParsedMetadata(title, Trim4(Trim(parse.Channel)));
    }
}