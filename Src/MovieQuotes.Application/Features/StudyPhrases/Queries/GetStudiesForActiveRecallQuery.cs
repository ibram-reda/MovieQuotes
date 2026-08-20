namespace MovieQuotes.Application.Features.StudyPhrases.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

public class GetStudiesForActiveRecallQuery : IRequest<OperationPageResult<StudyPhrase>>
{
}
