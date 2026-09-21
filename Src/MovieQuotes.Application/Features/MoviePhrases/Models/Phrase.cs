namespace MovieQuotes.Application.Features.MoviePhrases.Models;

public class Phrase
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string MovieName { get; set; } = string.Empty;
    public string VideoLocation { get; set; } = string.Empty;
    public string MovieCoverUrl {get; set;} = "";

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }


}
