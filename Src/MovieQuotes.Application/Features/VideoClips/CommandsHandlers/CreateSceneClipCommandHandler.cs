namespace MovieQuotes.Application.Features.VideoClips.CommandsHandlers;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


internal class CreateSceneClipCommandHandler : IRequestHandler<CreateSceneClipCommand, OperationResult<string>>
{

    private readonly MovieQuotesDbContext dbContext;


    public CreateSceneClipCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public Task<OperationResult<string>> Handle(CreateSceneClipCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
