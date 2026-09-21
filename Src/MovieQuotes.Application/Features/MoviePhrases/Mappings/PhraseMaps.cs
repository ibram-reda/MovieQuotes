namespace MovieQuotes.Application.Features.MoviePhrases.Mappings;

using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Domain.Models;

internal static class PhraseMaps
{
    internal static Phrase ToPhrase(this SubtitlePhrase src,string CashPath)
    {
        return new Phrase
        {
            Id = src.Id, 
            Text = src.Text, 
            StartTime = src.StartTime,
            EndTime = src.EndTime, 
            MovieName = src.Movie?.Title ?? string.Empty, 
            VideoLocation = src.GetVideoClipPath(CashPath) ?? string.Empty,
            MovieCoverUrl = src.Movie?.GetCoverUrl()??""
        };
    }
}
