namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Models;
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
            result.AddError(Enums.ErrorCode.ValidationError, MoviePhrasesMessages.RequiredMovieId);
        if (!File.Exists(request.SubtitleLocation))
            result.AddError(Enums.ErrorCode.ValidationError, MoviePhrasesMessages.RequiredValidSubtitleLocation);
        #endregion

        var movie = await this.dbContext.Movies.Include(a=>a.Subtitles).FirstOrDefaultAsync(a => a.Id == request.MovieId);
        if (movie is null)
        {
            result.AddError(Enums.ErrorCode.NotFound, MoviePhrasesMessages.MovieNotFound, request.MovieId);
            return result;
        }
        await movie.AddSubtitleFromFileAsync(request.SubtitleLocation);

        await this.dbContext.SaveChangesAsync();

        await AddWordsAsync(movie.Subtitles);

        return result;
    }

    private async Task AddWordsAsync(List<SubtitlePhrase> phrases)
    {
        foreach (var phrase in phrases)
        {
            if (phrase.PhraseWords.Any()) continue;
            var words = phrase.Text.Split(' ');
            int i = 0;
            foreach (var word in words)
            {
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
