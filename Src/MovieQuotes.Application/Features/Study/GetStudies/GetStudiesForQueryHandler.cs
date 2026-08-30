namespace MovieQuotes.Application.Features.Study;

using MediatR;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

internal class GetStudiesQueryHandler : IRequestHandler<GetStudiesQuery, OperationPageResult<StudyPhrase>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetStudiesQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationPageResult<StudyPhrase>> Handle(GetStudiesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyPhrase>();

        var baseQuery = this.dbContext.CardProgresses
                            .Where(p=>p.NextReviewAt <= DateTime.Now)
                            .Where(p=> p.ExerciseType == StudyExerciseType.ContextRecall)
                            .Include(p=>p.Card)
                            .ThenInclude(card=>card.StudyMaterial)
                            .Where(p=>p.Card.IsActive)
                            .Select(progress=>new StudyPhrase
                            {
                                ProgressId = progress.Id,
                                StudyCardId = progress.Card.Id,
                                PhraseText = progress.Card.StudyMaterial.Phrase!.Text,
                                Content = progress.Card.StudyMaterial.Content,
                                Origin = progress.Card.StudyMaterial.Origin,
                                Definition = progress.Card.StudyMaterial.Definition,
                                Synonyms = progress.Card.StudyMaterial.Synonyms,
                                Examples = progress.Card.StudyMaterial.Examples,
                                VideoPath = progress.Card.StudyMaterial.Phrase.GetVideoClipPath()??""
                            });
            

        var qry = baseQuery;

        var totalCount = await qry.CountAsync();

        result.Payload = await qry.Take(request.ItemPerPage).ToListAsync();
        result.Count = totalCount;
        result.CurrentPageNumber = 1;
        result.HasNext = totalCount > request.ItemPerPage;

        return result;
    }
}