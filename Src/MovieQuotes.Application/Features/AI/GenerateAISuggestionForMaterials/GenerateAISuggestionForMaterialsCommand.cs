namespace MovieQuotes.Application.Features.AI.GenerateAISuggestionForMaterials;

using MediatR;
using MovieQuotes.AI;
using MovieQuotes.Application.Common.Models;

public sealed class GenerateAISuggestionForMaterialsCommand : IRequest<OperationResult<bool>>
{
    public GenerateAISuggestionForMaterialsCommand()
    {
    }

}