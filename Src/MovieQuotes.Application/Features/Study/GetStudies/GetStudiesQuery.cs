namespace MovieQuotes.Application.Features.Study;

using MediatR;
using MovieQuotes.Application.Common.Models; 

public class GetStudiesQuery : IRequest<OperationPageResult<StudyPhrase>>
{
    public int ItemPerPage { get; set; }   = 400;
    public int PageNumber { get; set; } = 1;
}