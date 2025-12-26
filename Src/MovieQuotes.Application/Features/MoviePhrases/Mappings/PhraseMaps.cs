namespace MovieQuotes.Application.Features.MoviePhrases.Mappings;

using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Domain.Models;

internal static class PhraseMaps
{
    internal static Phrase ToPhrase(this SubtitlePhrase src)
    {
        return new Phrase
        {
            Id = src.Id,
            Sequence = src.Sequence,
            Text = src.Text, 
            StartTime = src.StartTime,
            EndTime = src.EndTime,
            Duration = src.Duration,
            MovieName = src.Movie?.Title ?? string.Empty,
            MoviePath = Path.Combine(src.Movie?.BaseFolderDir??"",src.Movie?.FolderName??""),
            VideoLocation = src.GetVideoClipPath() ?? string.Empty
        };
    }
}
