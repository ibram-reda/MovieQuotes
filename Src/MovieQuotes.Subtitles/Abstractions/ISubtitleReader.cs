namespace MovieQuotes.Subtitles.Abstractions;

using MovieQuotes.Subtitles.Models;

public interface ISubtitleReader
{
    Task<IReadOnlyList<SubtitleEntry>> Read(Stream stream);
}