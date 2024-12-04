using MediatR;
using MovieQuotes.Application.Models;

namespace MovieQuotes.Application.Operations.Commands;

public class CleanDatabaseCommand : IRequest<OperationResult<string>>
{
}
