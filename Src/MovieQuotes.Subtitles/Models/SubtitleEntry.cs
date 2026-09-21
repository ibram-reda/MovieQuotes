namespace MovieQuotes.Subtitles.Models;

public sealed record SubtitleEntry(
    int Index,
    TimeSpan Start,
    TimeSpan End,
    string Text);