namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Services;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

internal class InsertPhrasesForMovieCommandHandler : IRequestHandler<InsertPhrasesForMovieCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public InsertPhrasesForMovieCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(InsertPhrasesForMovieCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        #region valdition
        if (request.MovieId <= 0)
            result.AddError(ErrorCode.ValidationError, MoviePhrasesMessages.RequiredMovieId);
        #endregion

        var movie = await unitOfWork.Movies.Query.Include(a => a.Subtitles).FirstOrDefaultAsync(a => a.Id == request.MovieId);
        if (movie is null)
        {
            result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.MovieNotFound, request.MovieId);
            return result;
        }

        // if subtitle is not provided, try to find it in the movie folder
        if (string.IsNullOrWhiteSpace(request.SubtitleLocation))
        {
            var baseFolder = Path.Combine(movie.BaseFolderDir, movie.FolderName);
            if (string.IsNullOrEmpty(baseFolder))
            {
                result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.MovieBaseFolderNotFound);
                return result ;
            }
            var subtitleFolder = Path.Combine(baseFolder, "subtitles");
            if (!Directory.Exists(subtitleFolder))
                subtitleFolder = Path.Combine(baseFolder, "Subs");
            if (!Directory.Exists(subtitleFolder))
            {
                result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.SubtitleFolderNotFound, baseFolder);
                return result;
            }
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

        await this.unitOfWork.SaveAsync();

        //await AddWordsAsync(movie.Subtitles);

        return result;
    }

    private async Task AddWordsAsync(List<SubtitlePhrase> phrases)
    {
        foreach (var phrase in phrases)
        {
            if (phrase.PhraseWords.Any()) continue;
            var words = phrase.GetWords();
            int i = 0;
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;
                var w = await unitOfWork.Words.Query.FirstOrDefaultAsync(a => a.Text == word);
                if (w is null)
                {
                    w = Word.CreateWord(word);
                    await unitOfWork.Words.AddAsync(w);
                    await unitOfWork.SaveAsync();
                }
                var pw = PhraseWords.Create(phrase, w, i++);
                phrase.PhraseWords.Add(pw);
            }
            await unitOfWork.SaveAsync();
        }
    }


}
