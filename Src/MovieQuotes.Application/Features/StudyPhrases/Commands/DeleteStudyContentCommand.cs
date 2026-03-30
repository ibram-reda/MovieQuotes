namespace MovieQuotes.Application.Features.StudyPhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

public class DeleteStudyContentCommand : IRequest<OperationResult<int>>
{ 
    public DeleteStudyContentCommand(int StudyId)
    {
        StudyContentId = StudyId;
    }
    public int StudyContentId { get; set; }
}
