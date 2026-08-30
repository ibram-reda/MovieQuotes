namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Models;

public class GetStudyMaterialsQuery : IRequest<OperationPageResult<StudyMaterial>>
{
    public int MovieId { get; init; }
    public string? Level { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? SearchText { get; init; }
    public uint ResultPerPage { get; init; } = 10;
    public uint PageNumber { get; init; }
}