namespace MovieQuotes.Subtitles.Models;

public sealed record SubtitleBadToken(string Value,string Proplem)
:SubtitleToken(SubtitleTokenType.BadToken,Value);

