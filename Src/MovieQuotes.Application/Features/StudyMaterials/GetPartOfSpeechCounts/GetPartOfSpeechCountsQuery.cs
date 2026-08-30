namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Models;

public class GetPartOfSpeechCountsQuery : IRequest<OperationPageResult<PartOfSpeechCountDto>>
{
    public GetPartOfSpeechCountsQuery(int movieId)
    {
        MovieId = movieId;
    }

    public int MovieId { get; }
}