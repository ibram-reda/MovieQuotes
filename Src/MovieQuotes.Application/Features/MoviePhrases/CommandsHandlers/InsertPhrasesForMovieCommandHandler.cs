namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Services;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

internal class InsertPhrasesForMovieCommandHandler : IRequestHandler<InsertPhrasesForMovieCommand, OperationResult<bool>>
{
    private readonly MovieQuotesDbContext dbContext;

    public InsertPhrasesForMovieCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<bool>> Handle(InsertPhrasesForMovieCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        #region valdition
        if (request.MovieId <= 0)
            result.AddError(ErrorCode.ValidationError, MoviePhrasesMessages.RequiredMovieId);
        #endregion

        var movie = await this.dbContext.Movies.Include(a => a.Subtitles).FirstOrDefaultAsync(a => a.Id == request.MovieId);
        if (movie is null)
        {
            result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.MovieNotFound, request.MovieId);
            return result;
        }

        // if subtitle is not provided, try to find it in the movie folder
        if (string.IsNullOrWhiteSpace(request.SubtitleLocation))
        {
            var baseFolder = Path.GetDirectoryName(movie.LocalPath);
            var subtitleFolder = Path.Combine(baseFolder ?? "", "subtitles");
            var enSubLocation = Directory.GetFiles(subtitleFolder).FirstOrDefault(f => f.EndsWith("en.srt"));
            if (!File.Exists(enSubLocation))
                result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.SubtitleFileNotFound);
            else
                request.SubtitleLocation = enSubLocation;
        }

        if (!File.Exists(request.SubtitleLocation))
        {
            result.AddError(ErrorCode.ValidationError, MoviePhrasesMessages.RequiredValidSubtitleLocation);
            return result;
        }

        var loadingResult = await SubtitleManager.LoadAsync(request.SubtitleLocation);
        if (loadingResult.IsError)
            result.AddErrorRange(loadingResult.Errors);

        loadingResult.Payload?.RemoveMarkupAndDuplicateSpaces();

        movie.AddSubtitlesFromList(loadingResult.Payload!.ToList());

        if (result.IsError) return result;

        await this.dbContext.SaveChangesAsync();

        await AddWordsAsync(movie.Subtitles);

        return result;
    }

    private async Task AddWordsAsync(List<SubtitlePhrase> phrases)
    {
        foreach (var phrase in phrases)
        {
            if (phrase.PhraseWords.Any()) continue;
            var words = phrase.Text.Split(' ', '\n', ',', '!', '.');
            int i = 0;
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;
                var normalizedWord = Normalize(word);
                var w = await dbContext.Word.FirstOrDefaultAsync(a => a.Text == normalizedWord);
                if (w is null)
                {
                    w = Word.CreateWord(normalizedWord);
                    dbContext.Word.Add(w);
                    await dbContext.SaveChangesAsync();
                }
                var pw = PhraseWords.Create(phrase, w, i++);
                phrase.PhraseWords.Add(pw);
            }
            await dbContext.SaveChangesAsync();
        }
    }

    Regex rgx = new Regex("^[^a-zA-Z0-9]+|[^a-zA-Z0-9]+$");
    private string Normalize(string word)
    {
        return rgx.Replace(word, "");
    }
}
