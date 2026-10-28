namespace MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GetSubtitlePhraseByIdQuery
{
    public GetSubtitlePhraseByIdQuery(int id)
    {
        this.PhraseId = id;
    }
    public int PhraseId { get; private set; }
}
