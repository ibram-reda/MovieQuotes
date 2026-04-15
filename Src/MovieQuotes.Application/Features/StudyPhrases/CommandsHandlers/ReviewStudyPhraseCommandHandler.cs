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

             // If no progress record exists for the given StudyPhraseId, it means the phrase has not been reviewed before.
             // In this case, we can create a new progress record with the initial review data.
             studyPhraseProgress = Domain.Models.StudyPhraseProgress.Create(request.StudyPhraseId);
             await _unitOfWork.StudyPhraseProgress.AddAsync(studyPhraseProgress);
             await _unitOfWork.SaveAsync(cancellationToken);
        }

        studyPhraseProgress.Review(request.Quality);
        await _unitOfWork.StudyPhraseProgress.UpdateAsync(studyPhraseProgress);
        await _unitOfWork.SaveAsync(cancellationToken);

        result.Payload = studyPhraseProgress.NextReviewDate;
        return result;
 
    }
}
