namespace MovieQuotes.Application.Features.MoviePhrases;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal static class MoviePhrasesMessages
{
    public const string PageNotFound = "Not found";
    public const string MovieNotFound = "movie with id {0} is not found";
    public const string PhraseNotFound = "phrase with id {0} is not found";
    public const string RequiredMovieId = "movie id is required"; 
    public const string RequiredValidSubtitleLocation = "Valid Subtitle Location is required";
    public const string SubtitleFileNotFound = "Subtitle file not found in default location.";
    public const string SubtitleFolderNotFound = "subtitles/subs Folder not found in '{0}' Location";
    public const string SearchTextEmpty = "Search text cannot be empty.";

    public const string MovieBaseFolderNotFound = "Can Not Locate Base folder Url for '{0}' Movie";
}
