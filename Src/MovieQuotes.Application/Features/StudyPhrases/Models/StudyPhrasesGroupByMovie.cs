namespace MovieQuotes.Application.Features.StudyPhrases.Models;

public class StudyPhrasesGroupByMovie
{
    public int MovieId { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public string MovieCoverUrl { get; set; } = string.Empty;

    public int StudyCount { get; set; }


    public override string ToString() => MovieName;
}
