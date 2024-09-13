namespace MovieQuotes.Api.MappingProfiles;

using AutoMapper;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Models;

public class PhrasesMappings : Profile
{
    public PhrasesMappings()
    {
        string baseUrl = "https://localhost:7079/Video/";

        CreateMap<Phrase, PhraseResponse>()
            .ForMember(a => a.VideoUrl, opt => opt.MapFrom(src => $"{baseUrl}{src.Id}"));
    }
}
