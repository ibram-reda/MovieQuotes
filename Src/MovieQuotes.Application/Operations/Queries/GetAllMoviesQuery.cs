namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GetAllMoviesQuery : IRequest<OperationResult<List<MovieInfo>>>
{
}
