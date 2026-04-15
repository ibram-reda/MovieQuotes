namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class IsMovieExistQueryHandler : IRequestHandler<IsMovieExistQuery, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    public IsMovieExistQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(IsMovieExistQuery request, CancellationToken cancellationToken)
    {
        OperationResult<bool> res = new();

        var isExist = await unitOfWork.Movies.Query.AnyAsync(m => m.FolderName == request.FolderName);
        res.Payload = isExist;
        return res;
    }
}
