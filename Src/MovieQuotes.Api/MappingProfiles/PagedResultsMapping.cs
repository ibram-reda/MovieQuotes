namespace MovieQuotes.Api.MappingProfiles;

using AutoMapper;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;

public class PagedResultsMapping : Profile
{
    public PagedResultsMapping()
    {
        CreateMap<OperationPageResult<Phrase>, PagedResponse<PhraseResponse>>();
    }
}
