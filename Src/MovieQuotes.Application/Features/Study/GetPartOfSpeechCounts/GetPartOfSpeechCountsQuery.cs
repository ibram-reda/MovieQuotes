namespace MovieQuotes.Application.Features.Study;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class GetPartOfSpeechCountsQuery : IRequest<OperationPageResult<PartOfSpeechCountDto>>
{

}