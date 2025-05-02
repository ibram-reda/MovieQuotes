namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Services;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal class GetAllPhrasesForMovieQueryHandlers : IRequestHandler<GetAllPhrasesForMovieQuery, OperationResult<List<Phrase>>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllPhrasesForMovieQueryHandlers(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<List<Phrase>>> Handle(GetAllPhrasesForMovieQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<Phrase>>();

        var src = request.Language switch
        {
            Language.ar => await GetArabicFromDesk(request.MovieId, result),
            Language.en => this.dbContext.SubtitlePhrases
            .Where(a => a.MovieId == request.MovieId)
        };

        var load =   src
            .OrderBy(a=>a.StartTime)
            .Select(a => new Phrase()
            {
                Id = a.Id,
                Sequence = a.Sequence,
                Text = a.Text,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Duration = a.Duration,
            });

        result.Payload = request.Language switch
        {
            Language.ar => load.ToList(),
            Language.en => await load.ToListAsync()
        };
           

        return result;
    }

    async Task<IQueryable<SubtitlePhrase>> GetArabicFromDesk(int MovieId, OperationResult<List<Phrase>> result)
    {
        var movie = await this.dbContext.Movies.FirstOrDefaultAsync(a => a.Id == MovieId);

        if (movie is null)
            result.AddError(ErrorCode.NotFound, "Movie not found");
        var b =  Path.GetDirectoryName(movie.LocalPath);
        var subFolder = Path.Combine(b, "subtitles");
        string? file = null;
        if (!Directory.Exists(subFolder))
            result.AddError(ErrorCode.NotFound, "cannot find the subtitles folder");
        else
            file = Directory.GetFiles(subFolder).FirstOrDefault(f => f.EndsWith("ar.srt"));
        
        if (file is null)
            result.AddError(ErrorCode.NotFound, "can't load Arabic subtitle file maybe not exist or not end with 'ar.srt'");

        var subLoadResult = await SubtitleManager.LoadAsync(file);
        if (subLoadResult.IsError)
            result.AddErrorRange(subLoadResult.Errors);

        return subLoadResult.Payload ?? Enumerable.Empty<SubtitlePhrase>().AsQueryable();

    }
}
