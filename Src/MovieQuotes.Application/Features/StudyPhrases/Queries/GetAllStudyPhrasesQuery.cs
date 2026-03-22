namespace MovieQuotes.Application.Features.StudyPhrases.Queries;

using System.ComponentModel;
using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;


public class GetAllStudyPhrasesQuery : IRequest<OperationPageResult<StudyPhrase>>
{

    /// <summary>
    /// if MovieId is set then it will return study phrase for that movie
    /// otherwise it will return all study phrases
    /// </summary>
    public int MovieId { get; set; } = 0;

    /// <summary>
    /// if true then it will return only the phrases that are due for review 
    /// (NextReviewDate <= DateTime.Now)
    /// </summary>
    public bool OnlyDuePhrases { get; set; } = false;
}
