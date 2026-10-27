namespace MovieQuotes.Subtitles.Models;

public sealed record SubtitleRun(
    string Text,
    bool Bold = false,
    bool Italic = false,
    bool Underline = false,
    string? FontFamily = null,
    double? FontSize = null,
    string? Foreground = null,
    bool IsLineBreak = false);
