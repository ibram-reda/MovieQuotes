namespace MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;

public class UpdateStudyProgressCommand
{
    public UpdateStudyProgressCommand(int progressId, int reviewQuality)
    {
        ProgessId = progressId;
        ReviewQuality = reviewQuality;
    }

    public int ProgessId { get; }

    public int ReviewQuality { get; }
}
