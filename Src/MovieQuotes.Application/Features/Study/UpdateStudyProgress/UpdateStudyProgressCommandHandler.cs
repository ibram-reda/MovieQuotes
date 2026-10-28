namespace MovieQuotes.Application.Features.Study.UpdateStudyProgress;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;

public class UpdateStudyProgressCommandHandler
{
    private readonly MovieQuotesDbContext dbContext;

    public UpdateStudyProgressCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<bool>> Handle(UpdateStudyProgressCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        if (request is null || request.ProgessId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "A valid study card Progress id is required.");
            return result;
        }


        try
        {
            var progress = await this.dbContext.CardProgresses
                .SingleOrDefaultAsync(p=>p.Id == request.ProgessId, cancellationToken);

            if (progress is null)
            {
               result.AddError(ErrorCode.NotFound,"progress with id {0} not foudn",request.ProgessId);
               return result;
            }

            progress.Review((ReviewQuality)request.ReviewQuality);
            await this.dbContext.SaveChangesAsync(cancellationToken);

            result.Payload = true;
        }
        catch (Exception exception)
        {
            result.AddException(exception);
        }

        return result;
    }
}
