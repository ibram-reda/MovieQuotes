namespace MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterialLevels;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

public sealed class GetStudyMaterialLevelsQueryHandler
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetStudyMaterialLevelsQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<IReadOnlyList<LevelFilter>>> Handle(
        GetStudyMaterialLevelsQuery request,
        CancellationToken cancellationToken)
    {
        var levels = await unitOfWork.StudyMaterials.Query
            .AsNoTracking()
            .GroupBy(material => material.Level ?? string.Empty)
            .Select(g => new
            {
                Level = g.Key,
                Count = g.Count()
            }).OrderBy(l => l.Level)
            .ToListAsync(cancellationToken);

        var lvl = levels.Select(l=>new LevelFilter(l.Level,l.Count)).ToList();
           
        
        lvl.Insert(0, new LevelFilter("Any Level",levels.Sum(l=>l.Count)));

        return new OperationResult<IReadOnlyList<LevelFilter>>
        {
            Payload =  lvl
        };
    }
}
