namespace MovieQuotes.Application.Features.Study.GetStudies;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

public class GetStudiesQueryHandler
{
    private readonly MovieQuotesDbContext dbContext;
    private readonly AppSettings settings;

    public GetStudiesQueryHandler(MovieQuotesDbContext dbContext, AppSettings settings)
    {
        this.dbContext = dbContext;
        this.settings = settings;
    }

    public async Task<OperationPageResult<StudyPhrase>> Handle(GetStudiesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyPhrase>();

        var baseQuery = this.dbContext.CardProgresses
                            .Where(p => p.NextReviewAt <= DateTime.Now);

        if (request.StudyType == StudyType.ContextRecall)
            baseQuery = baseQuery
                   .Where(p => p.ExerciseType == StudyExerciseType.ContextRecall);

        else if (request.StudyType == StudyType.Recognition)
            baseQuery = baseQuery
                            .Where(p => p.ExerciseType == StudyExerciseType.Recognition);


        var qry = baseQuery.Include(p => p.Card)
                          .ThenInclude(card => card.StudyMaterial)
                          .ThenInclude(material => material.Phrases)
                          .ThenInclude(MaterialPhrase=>MaterialPhrase.Phrase)
                          .Where(p => p.Card.IsActive)
                          .Select(progress => new StudyPhrase
                          { 
                              MaterialId = progress.Card.StudyMaterialId,
                              StudyCardId = progress.Card.Id,
                              ProgressId = progress.Id,
                              StudyType = progress.Card.StudyMaterial.PartOfSpeech, 
                              Content = progress.Card.StudyMaterial.Content,
                              Origin = progress.Card.StudyMaterial.Origin,
                              Definition = progress.Card.StudyMaterial.Definition,
                              Synonyms = progress.Card.StudyMaterial.Synonyms,
                              Examples = progress.Card.StudyMaterial.Examples,
                              
                              MoviePhrase = new(progress.Card.StudyMaterial.Phrases.OrderBy(p => EF.Functions.Random()).First().Phrase,settings.VideoCashPath) 
                          });

        var totalCount = await qry.CountAsync();

        result.Payload = await qry.Take(request.ItemPerPage).ToListAsync();
        result.Count = totalCount;
        result.CurrentPageNumber = 1;
        result.HasNext = totalCount > request.ItemPerPage;

        return result;
    }
}
