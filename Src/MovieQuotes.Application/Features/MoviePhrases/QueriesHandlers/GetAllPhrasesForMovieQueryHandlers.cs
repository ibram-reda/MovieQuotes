namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Services;
using MovieQuotes.Application.Features.MoviePhrases.Mappings;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class GetAllPhrasesForMovieQueryHandlers : IRequestHandler<GetAllPhrasesForMovieQuery, OperationResult<List<Phrase>>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetAllPhrasesForMovieQueryHandlers(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<List<Phrase>>> Handle(GetAllPhrasesForMovieQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<Phrase>>();

        var src = request.Language switch
        {
            Language.ar => await GetArabicFromDesk(request.MovieId, result),
            Language.en => unitOfWork.SubtitlePhrases.Query
            .Where(a => a.MovieId == request.MovieId),
            _ => throw new NotSupportedException($"Language {request.Language} is not supported.")
        };

        var load = src
            .OrderBy(a => a.StartTime)
            .Select(a => a.ToPhrase());

        result.Payload = request.Language switch
        {
            Language.ar => load.ToList(),
            Language.en => await load.ToListAsync(),
            _ => throw new NotSupportedException($"Language {request.Language} is not supported.")
        };


        return result;
    }

    async Task<IQueryable<SubtitlePhrase>> GetArabicFromDesk(int MovieId, OperationResult<List<Phrase>> result)
    {
        var movie = await unitOfWork.Movies.Query.FirstOrDefaultAsync(a => a.Id == MovieId);

        if (movie is null)
            result.AddError(ErrorCode.NotFound, "Movie not found");
        var b = Path.Combine(movie!.BaseFolderDir, movie.FolderName);
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
