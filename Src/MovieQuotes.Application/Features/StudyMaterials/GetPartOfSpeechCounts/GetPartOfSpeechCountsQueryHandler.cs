namespace MovieQuotes.Application.Features.StudyMaterials;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;
using Org.BouncyCastle.Ocsp;

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
        var qury = unitOfWork.StudyMaterials.Query
            .AsNoTracking();

        if(request.MovieId > 0)
          qury =  qury.Where(a=>a.Phrases.Any(a=>a.Phrase.Movie!.Id == request.MovieId));

        var counts = await qury.GroupBy(material => material.PartOfSpeech ?? string.Empty)
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
