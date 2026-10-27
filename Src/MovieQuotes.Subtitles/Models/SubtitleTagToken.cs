namespace MovieQuotes.Subtitles.Models;

public sealed record SubtitleTagToken(
    string Value,
    string Name,
    bool IsClosing,
    bool IsSelfClosing,
    IReadOnlyDictionary<string, string> Attributes,
    IReadOnlyList<SubtitleToken> Children,
    string? ClosingValue = null)
    : SubtitleToken(SubtitleTokenType.Tag, Value);
