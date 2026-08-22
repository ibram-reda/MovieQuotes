namespace MovieQuotes.Application.Features.Study;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

internal sealed class GetPartOfSpeechCountsQueryHandler : IRequestHandler<GetPartOfSpeechCountsQuery, OperationPageResult<PartOfSpeechCountDto>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetPartOfSpeechCountsQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationPageResult<PartOfSpeechCountDto>> Handle(
        GetPartOfSpeechCountsQuery request,
        CancellationToken cancellationToken)
    {
        var counts = await unitOfWork.StudyMaterials.Query
            .AsNoTracking()
            .GroupBy(material => material.PartOfSpeech ?? string.Empty)
            .Select(group => new PartOfSpeechCountDto{
                PartOfSpeech = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(item => item.Count)
            .ToListAsync(cancellationToken);

        return new OperationPageResult<PartOfSpeechCountDto>
        {
            Payload = counts,
            Count = counts.Count,
            HasNext = false,
            CurrentPageNumber = 1,
            ItemPerPage = (uint)counts.Count
        };
    }
}
