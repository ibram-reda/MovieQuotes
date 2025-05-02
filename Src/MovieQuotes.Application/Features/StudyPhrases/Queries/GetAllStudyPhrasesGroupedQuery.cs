using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

namespace MovieQuotes.Application.Features.StudyPhrases.Queries;

public class GetAllStudyPhrasesGroupedQuery : IRequest<OperationPageResult<StudyPhrasesGroupByMovie>>
{
}
