namespace MovieQuotes.Subtitles.Abstractions;

using MovieQuotes.Subtitles.Models;

public interface ISubtitleParser
{
    SubtitleDocument Parse(string subtitleText);
}