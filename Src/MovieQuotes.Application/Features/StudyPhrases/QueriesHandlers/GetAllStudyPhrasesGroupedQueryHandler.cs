namespace MovieQuotes.Application.Features.StudyPhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Infrastructure;

internal class GetAllStudyPhrasesGroupedQueryHandler : IRequestHandler<GetAllStudyPhrasesGroupedQuery, OperationPageResult<StudyPhrasesGroupByMovie>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllStudyPhrasesGroupedQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationPageResult<StudyPhrasesGroupByMovie>> Handle(GetAllStudyPhrasesGroupedQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyPhrasesGroupByMovie>();

        var query = this.dbContext
                        .StudyPhrases
                        .GroupBy(a => new
                        {
                            Name = a.Phrase!.Movie!.Title,
                            Id = a.Phrase.MovieId,
                            BaseFolderDir = a.Phrase.Movie.BaseFolderDir,
                            FolderName = a.Phrase.Movie.FolderName,
                            Cover = a.Phrase.Movie.CoverFilePath,
                        })
                        .Select(grp => new StudyPhrasesGroupByMovie
                        {
                            MovieName = grp.Key.Name,
                            MovieId = grp.Key.Id,
                            MovieCoverUrl = Path.Combine(grp.Key.BaseFolderDir, grp.Key.FolderName, grp.Key.Cover),
                            StudyCount = grp.Count()
                        });

        result.Count = await query.CountAsync();
        result.ItemPerPage = (uint)result.Count;
        result.CurrentPageNumber = 1;
        result.HasNext = false;
        result.Payload = await query.ToListAsync();

        return result;
    }
}
