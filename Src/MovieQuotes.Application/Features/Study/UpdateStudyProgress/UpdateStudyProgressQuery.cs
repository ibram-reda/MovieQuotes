namespace MovieQuotes.Application.Features.Study;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;

public class UpdateStudyProgressQuery : IRequest<OperationResult<bool>>
{
    public UpdateStudyProgressQuery(int progressId, int reviewQuality)
    {
        ProgessId = progressId;
        ReviewQuality = reviewQuality;
    }

    public int ProgessId { get; }

    public int ReviewQuality { get; }
}
