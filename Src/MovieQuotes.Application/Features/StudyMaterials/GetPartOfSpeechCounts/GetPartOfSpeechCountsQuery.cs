namespace MovieQuotes.Application.Features.StudyMaterials.GetPartOfSpeechCounts;
using MovieQuotes.Application.Common.Models;

public class GetPartOfSpeechCountsQuery
{
    public GetPartOfSpeechCountsQuery(int movieId)
    {
        MovieId = movieId;
    }

    public int MovieId { get; }
}
