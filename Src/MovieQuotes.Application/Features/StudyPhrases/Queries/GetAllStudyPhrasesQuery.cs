namespace MovieQuotes.Application.Features.StudyPhrases.Queries;

using MediatR;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Models;


public class GetAllStudyPhrasesQuery : IRequest<OperationPageResult<StudyPhrase>>
{
}
