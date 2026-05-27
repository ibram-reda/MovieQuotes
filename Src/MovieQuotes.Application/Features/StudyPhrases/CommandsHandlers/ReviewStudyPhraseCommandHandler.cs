namespace MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Mappings;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Domain.Interfaces;

public class ReviewStudyPhraseCommandHandler : IRequestHandler<ReviewStudyPhraseCommand, OperationResult<DateTime>>
{
    private readonly IMovieQUnitOfWork _unitOfWork;

    public ReviewStudyPhraseCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<DateTime>> Handle(ReviewStudyPhraseCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<DateTime>();

        if (request.Quality < 0 || request.Quality > 5)
        {
            result.AddError(ErrorCode.InvalidInput, "Quality must be between 0 and 5.");
            return result;
        }

        var studyPhraseProgress = await _unitOfWork.StudyPhraseProgress
        .GetProgressForPhrase(request.StudyPhraseId);

        if (studyPhraseProgress == null)
        {

             // throw new InvalidOperationException($"No progress found for StudyPhraseId {request.StudyPhraseId}");
            result.AddError(ErrorCode.NotFound, $"No progress found for StudyPhraseId {request.StudyPhraseId}");
            return result;
        }

        studyPhraseProgress.Review(request.Quality);
        await _unitOfWork.StudyPhraseProgress.UpdateAsync(studyPhraseProgress);
        await _unitOfWork.SaveAsync(cancellationToken);

        result.Payload = studyPhraseProgress.NextReviewDate;
        return result;
 
    }
}
