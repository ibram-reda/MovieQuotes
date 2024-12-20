namespace MovieQuotes.Api.MappingProfiles;

using AutoMapper;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Models;

public class PagedResultsMapping : Profile
{
    public PagedResultsMapping()
    {
        CreateMap<OperationPageResult<Phrase>, PagedResponse<PhraseResponse>>();
    }
}
