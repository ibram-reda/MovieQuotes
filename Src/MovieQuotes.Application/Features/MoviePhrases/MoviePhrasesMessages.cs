namespace MovieQuotes.Application.Features.MoviePhrases;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal static class MoviePhrasesMessages
{
    public const string MovieNotFound = "movie with id {0} is not found";
    public const string RequiredMovieId = "movie id is required"; 
    public const string RequiredValidSubtitleLocation = "Valid Subtitle Location is required";
    public const string SubtitleFileNotFound = "Subtitle file not found in default location.";
}
