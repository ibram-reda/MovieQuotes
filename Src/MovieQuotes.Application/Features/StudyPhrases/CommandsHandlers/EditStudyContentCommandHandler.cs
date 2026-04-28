namespace MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Mappings;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Domain.Interfaces;

internal class EditStudyContentCommandHandler : IRequestHandler<EditStudyContentCommand, OperationResult<StudyPhrase>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    public EditStudyContentCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<OperationResult<StudyPhrase>> Handle(EditStudyContentCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyPhrase>();

        var studyPhrase = await unitOfWork.StudyPhrases.GetByIdAsync(request.StudyId);

        if (studyPhrase is null)
        {
            result.AddError(ErrorCode.NotFound, StudyPhraseMessages.PhraseNotFound, request.StudyId);
            return result;
        }

        studyPhrase.EditContent(request.Content);
        studyPhrase.EditTranslation(request.Translation);
        studyPhrase.EditStudyType(request.StudyType);
        studyPhrase.EditArContentTranslation(request.ArContentTranslation);
        studyPhrase.EditArPhraseTranslation(request.ArPhraseTranslation);
        studyPhrase.EditOrigin(request.Origin);
        studyPhrase.EditNotes(request.Notes);
        studyPhrase.EditExamples(request.Examples);
        studyPhrase.EditSynonyms(request.Synonyms);
        studyPhrase.EditLevel(request.Level);
        studyPhrase.EditPronunciation(request.Pronunciation);
        if(request.IsDraft)
            studyPhrase.MarkAsDraft();
        else
            studyPhrase.MarkAsReady();

        await unitOfWork.StudyPhrases.UpdateAsync(studyPhrase);
        var affectedRows = await unitOfWork.SaveAsync(cancellationToken);

        if (affectedRows <= 0)
        {
            result.AddError(ErrorCode.UpdateError, StudyPhraseMessages.FailedToUpdate);
            return result;
        }

        result.Payload = studyPhrase.ToStudyPhrase();
        return result;

    }
}
