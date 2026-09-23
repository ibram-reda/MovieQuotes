namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Models;


public class LevelFilter(string level, int count)
{
    public string Level { get; } = level;
    public int Count { get; } = count;
};
public sealed record GetStudyMaterialLevelsQuery
    : IRequest<OperationResult<IReadOnlyList<LevelFilter>>>;