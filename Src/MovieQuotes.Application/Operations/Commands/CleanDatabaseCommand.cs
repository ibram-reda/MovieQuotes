namespace MovieQuotes.Application.Operations.Commands;

using MediatR;
using MovieQuotes.Application.Models;

public class CleanDatabaseCommand : IRequest<OperationResult<string>>
{
}
