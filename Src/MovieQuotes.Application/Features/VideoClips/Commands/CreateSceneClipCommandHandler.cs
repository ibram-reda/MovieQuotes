namespace MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class CreateSceneClipCommandHandler
{

    private readonly IMovieQUnitOfWork unitOfWork;


    public CreateSceneClipCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public Task<OperationResult<string>> Handle(CreateSceneClipCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
