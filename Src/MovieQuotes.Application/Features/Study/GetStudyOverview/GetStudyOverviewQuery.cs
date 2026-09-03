namespace MovieQuotes.Application.Features.Study;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class GetStudyOverviewQuery : IRequest<OperationResult<StudyOverview>>
{
}
