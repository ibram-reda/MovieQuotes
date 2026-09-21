using MovieQuotes.Subtitles.Models;

namespace MovieQuotes.Subtitles.Abstractions;

public interface ISubtitleParser
{
    Task<IReadOnlyList<SubtitleEntry>> Parse(Stream stream);
}