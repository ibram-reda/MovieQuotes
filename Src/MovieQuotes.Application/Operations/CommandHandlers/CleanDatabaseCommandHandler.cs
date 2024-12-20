namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Infrastructure;

internal class CleanDatabaseCommandHandler : IRequestHandler<CleanDatabaseCommand, OperationResult<string>>
{
    private readonly MovieQuotesDbContext dbContext;

    public CleanDatabaseCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<string>> Handle(CleanDatabaseCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}
