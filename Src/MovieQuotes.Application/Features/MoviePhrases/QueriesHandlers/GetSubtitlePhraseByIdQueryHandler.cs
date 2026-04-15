namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Mappings;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


internal class GetSubtitlePhraseByIdQueryHandler : IRequestHandler<GetSubtitlePhraseByIdQuery, OperationResult<Phrase>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    public GetSubtitlePhraseByIdQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<OperationResult<Phrase>> Handle(GetSubtitlePhraseByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Phrase>();
        var phraseEntity = await unitOfWork.SubtitlePhrases.GetByIdAsync(request.PhraseId);
        if (phraseEntity is null)
        {
            result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.PhraseNotFound,request.PhraseId );
            return result;
        }

        result.Payload = phraseEntity.ToPhrase();

        return result;
    }
}
